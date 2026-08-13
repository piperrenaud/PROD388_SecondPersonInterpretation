using UnityEngine;
using System.Collections;

public class ProtagonistGridMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 3;
    [SerializeField] private float roomWidth = 7f;
    [SerializeField] private float roomDepth = 7f;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.75f;

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
            Debug.Log("Protagonist cant go outside the grid");
            return;
        }

        //find door connection
        DoorConnection connectingDoor = FindConnectingDoor(gridPosition, targetGridPosiion);

        if (connectingDoor == null)
        {
            Debug.LogWarning("No door found between " + gridPosition + " and " + targetGridPosiion);
            return;
        }


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

    private IEnumerator MoveThroughDoor(Vector2Int targetGridPosition, DoorConnection door)
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

        //close door
        yield return new WaitForSeconds(doorCloseDelay);
        door.CloseDoor();

        isMoving = false;
    }
}
