using System.Collections;
using UnityEngine;

public class ProtagonistMovement : MonoBehaviour
{
    [Header("To Start")]
    [SerializeField] private Transform pointOne;
    [SerializeField] private Transform startPoint;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.05f;
    [SerializeField] private Animator animator;

    [Header("Look")]
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float lookAroundDuration = 4f;
    [SerializeField] private float lookAroundAngle = 45f;

    [Header("Start DIalogue")]
    [SerializeField] private BranchingDialogue branchingDialogue;
    [SerializeField] private TerminalController terminalController;
    [SerializeField] private DialogueManager dialogueManager;

    private Coroutine lookAroundCoroutine;
    private Coroutine moveCoroutine;

    private void Start()
    {
        StartCoroutine(InitialPath());
    }

    private IEnumerator InitialPath()
    {
        yield return new WaitForSeconds(2f);

        MoveHere(pointOne);
        yield return new WaitForSeconds(2f);

        dialogueManager.SetProtagText("Oh no...");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        
        dialogueManager.SetProtagText("It's really bad in here");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        
        terminalController.TurnOnTerminal();
        dialogueManager.SetProtagText("Wait what's that...");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        MoveHere(startPoint);

        yield return new WaitForSeconds(2.5f);
        branchingDialogue.StartDialogue();
    }

    public void MoveHere(Transform location)
    {
        if (location == null) return;
        MoveHere(location.position);
    }

    public void MoveHere(Vector3 location)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveToPosition(location));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;

        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
        }

        while (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 newPosition = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime );

            newPosition.y = transform.position.y;
            transform.position = newPosition;

            yield return null;
        }

        transform.position = targetPosition;
        moveCoroutine = null;

        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
        }
    }

    public void StopMoving()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
        }
    }

    public bool IsMoving()
    {
        return moveCoroutine != null;
    }

    public void TurnTo(Transform target)
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        StartCoroutine(RotateToRotation(Quaternion.LookRotation(direction)));
    }

    public void LookAround()
    {
        if (lookAroundCoroutine != null)
        {
            StopCoroutine(lookAroundCoroutine);
        }

        lookAroundCoroutine = StartCoroutine(LookAroundRoutine());
    }

    private IEnumerator LookAroundRoutine()
    {
        Quaternion startingRotation = transform.rotation;

        Quaternion leftRotation = startingRotation * Quaternion.Euler(0f, -lookAroundAngle, 0f);
        Quaternion rightRotation = startingRotation * Quaternion.Euler(0f, lookAroundAngle, 0f);

        float elapsedTime = 0f;

        yield return RotateToRotation(leftRotation);
        yield return RotateToRotation(rightRotation);
        yield return RotateToRotation(leftRotation);
        yield return RotateToRotation(startingRotation);

        lookAroundCoroutine = null;
    }

    private IEnumerator RotateToRotation(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime );

            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
