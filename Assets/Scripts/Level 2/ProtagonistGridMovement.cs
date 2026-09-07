using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class ProtagonistGridMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Grid Settings")]
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 3;
    [SerializeField] private float roomWidth = 7f;
    [SerializeField] private float roomDepth = 7f;
    [TextArea(2, 4)]
    [SerializeField] private string outsideGrid = "Theres no door going that way.";

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.75f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Doors")]
    [SerializeField] private DoorConnection[] doors;
    [SerializeField] private float doorOpenDelay = 1f;
    [SerializeField] private float doorCloseDelay = 1f;

    //protagonists current room
    private Vector2Int gridPosition = new Vector2Int(0, 0);
    private bool isMoving = false;
    private Vector3 gridOrigin;

    private void Start()
    {
        //assumes protagonist starts in center of room
        gridOrigin = transform.position;

        animator.SetBool("IsRunning", false);
    }

    public void MoveForward()
    {
        TryMove(Vector3.forward);
    }

    public void MoveBackwards()
    {
        TryMove(Vector3.back);
    }

    public void MoveLeft()
    {
        TryMove(Vector3.left);
    }

    public void MoveRight()
    {
        TryMove(Vector3.right);
    }

    private void TryMove(Vector3 localDirection)
    {
        if (isMoving) return;

        //protagonists local space -> world space
        Vector3 worldDirection = transform.TransformDirection(localDirection);
        Vector2Int gridDirection = WorldDirectionToGridDirection(worldDirection);
        Vector2Int targetGridPosiion = gridPosition + gridDirection;

        if (targetGridPosiion.x < 0 ||
            targetGridPosiion.x >= columns ||
            targetGridPosiion.y < 0 ||
            targetGridPosiion.y >= rows)
        {
            dialogueManager.SetText(outsideGrid);
            return;
        }

        //find door connection
        DoorConnection connectingDoor = FindConnectingDoor(gridPosition, targetGridPosiion);

        if (connectingDoor == null)
        {
            dialogueManager.SetText(outsideGrid);
            return;
        }

        //check if door is blocked
        if (connectingDoor.IsBlocked)
        {
            dialogueManager.SetText(connectingDoor.BlockedDialogue);
            return;
        }

        Debug.Log(targetGridPosiion);

        StartCoroutine(MoveThroughDoor(targetGridPosiion, connectingDoor));
    }

    private DoorConnection FindConnectingDoor(Vector2Int from, Vector2Int to)
    {
        foreach (DoorConnection door in doors)
        {
            if (door != null && door.Connects(from, to))
            {
                return door;
            }
        }

        return null;
    }

    private Vector2Int WorldDirectionToGridDirection(Vector3 direction)
    {
        //direction = horizontal or vertical in worldspace?
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            return direction.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            return direction.z > 0 ? Vector2Int.up : Vector2Int.down;
        }
    }

    public IEnumerator MoveThroughDoor(Vector2Int targetGridPosition, DoorConnection door)
    {
        isMoving = true;

        //find direction
        Vector3 targetPosition =
                    gridOrigin + new Vector3(
                        targetGridPosition.x * roomWidth,
                        0f,
                        targetGridPosition.y * roomDepth);
        Vector3 direcitonToTarget = targetPosition - transform.position;
        direcitonToTarget.y = 0f;
        direcitonToTarget.Normalize();

        //turn to face next room
        if (direcitonToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direcitonToTarget);

            float rotationDuration = 0.2f;
            float elapsedRotation = 0f;
            Quaternion startRotation = transform.rotation;

            while (elapsedRotation < rotationDuration)
            {
                elapsedRotation += Time.deltaTime;
                float t = elapsedRotation / rotationDuration;

                transform.rotation = Quaternion.Slerp(
                    startRotation, targetRotation, t );

                yield return null;
            }

            transform.rotation = targetRotation;
        }

        //open door
        door.OpenDoor();
        yield return new WaitForSeconds(doorOpenDelay);

        //start running
        animator.SetBool("IsRunning", true);

        //move protagonist
        Vector3 startPositon = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            //smooth start/stop
            t = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(startPositon, targetPosition, t);

            yield return null;
        }

        transform.position = targetPosition;
        gridPosition = targetGridPosition;

        //stop running
        animator.SetBool("IsRunning", false);

        //close door
        yield return new WaitForSeconds(doorCloseDelay);
        door.CloseDoor();

        isMoving = false;
    }

    public Vector3? GetRandomValidDirection(out string directionName)
    {
        Vector3[] directions =
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        string[] directionNames =
        {
            "forward",
            "backward",
            "left",
            "right"
        };

        //store possible directions
        List<int> validDirections = new List<int>();

        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 worldDirection = transform.TransformDirection(directions[i]);

            Vector2Int gridDirection = WorldDirectionToGridDirection(worldDirection);

            Vector2Int targetGridPosition = gridPosition + gridDirection;

            //check grid boundaries
            if (targetGridPosition.x < 0 ||
                targetGridPosition.x >= columns ||
                targetGridPosition.y < 0 ||
                targetGridPosition.y >= rows)
            {
                continue;
            }

            //find door
            DoorConnection connectingDoor = FindConnectingDoor(gridPosition, targetGridPosition);

            if (connectingDoor == null) continue;

            //check blocked door
            if (connectingDoor.IsBlocked) continue;

            //this dir is valid
            validDirections.Add(i);
        }

        //no valid movement options
        if (validDirections.Count == 0)
        {
            directionName = null;
            return null;
        }

        //pick random valid direction
        int randomIndex = validDirections[Random.Range(0, validDirections.Count)];

        directionName = directionNames[randomIndex];

        return directions[randomIndex];
    }
}
