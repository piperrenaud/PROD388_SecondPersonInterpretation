using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ProtagMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Movement Points")]
    [SerializeField] private Transform[] movementPoints;
    [SerializeField] private OpenDoor firstDoor;
    [SerializeField] private OpenDoor lastDoor;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private float rotationSpeed = 8f;

    private int currentPointIndex = -1;
    private bool firstDoorTime = false;
    private bool lastDoorTime = false;  

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        agent.stoppingDistance = stoppingDistance;

        agent.updateRotation = false;

        agent.isStopped = true;
        if (animator != null) animator.SetBool("IsRunning", false);
    }

    private void Update()
    {
        if (agent == null || animator == null) return;

        if (!agent.isStopped && agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 direction = agent.velocity.normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime );
            }
        }

        if (animator != null)
        {
            bool isMoving = !agent.isStopped && agent.velocity.sqrMagnitude > 0.01f;
            animator.SetBool("IsRunning", isMoving);
        }
    }

    public void MoveToNextPoint()
    {
        if (movementPoints == null || movementPoints.Length == 0)
        {
            Debug.LogWarning($"{name} has no movement points assigned");
            return;
        }
        currentPointIndex++;

        if (currentPointIndex >= movementPoints.Length)
        {
            Debug.Log($"{name} has reached the final movement point");
            agent.isStopped = true;
            agent.ResetPath();

            if (animator != null) animator.SetBool("IsRunning", false);
            return;
        }

        MoveToPoint(movementPoints[currentPointIndex]);
    }

    public void MoveToPoint(int pointIndex)
    {
        if (movementPoints == null || movementPoints.Length == 0) return;

        if (pointIndex < 0 || pointIndex >= movementPoints.Length)
        {
            return;
        }

        currentPointIndex = pointIndex;

        MoveToPoint(movementPoints[pointIndex]);
    }

    private void MoveToPoint(Transform point)
    {
        if (point == null) return;

        agent.isStopped = false;
        agent.SetDestination(point.position);

        if (currentPointIndex == 0)
        {
            firstDoorTime = true;
            lastDoorTime = false;
            OpenFirstLastDoor();
            StartCoroutine(RoomOneRoutine());
        }

        if (currentPointIndex == 1) StartCoroutine(RoomTwoRoutine());
        if (currentPointIndex == 2) StartCoroutine(RoomThreeRoutine());
        if (currentPointIndex == 3) StartCoroutine(SwitchRoutine());
        if (currentPointIndex == 4)
        {
            lastDoorTime = true;
            firstDoorTime = false;
            OpenFirstLastDoor();
            StartCoroutine(EndRoutine());
        }
    }

    public void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();

        if (animator != null) animator.SetBool("IsRunning", false);
    }

    public bool HasReachedCurrentPoint()
    {
        if (currentPointIndex < 0 || currentPointIndex >= movementPoints.Length) return false;
        if (agent.pathPending) return false;

        return agent.remainingDistance <= agent.stoppingDistance;
    }

    public int GetCurrentPointIndex()
    {
        return currentPointIndex;
    }

    private void OpenFirstLastDoor()
    {
        if (firstDoor != null && firstDoorTime)
        {
            firstDoor.Open();
        }

        if (lastDoor != null && lastDoorTime)
        {
            lastDoor.Open();
        }
    }

    private IEnumerator RoomOneRoutine()
    {
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("Im in room 1");
    }

    private IEnumerator RoomTwoRoutine()
    {
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("Im in room 2");
    }

    private IEnumerator RoomThreeRoutine()
    {
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("Im in room 3");
    }

    private IEnumerator SwitchRoutine()
    {
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("Im switching sides");
    }

    private IEnumerator EndRoutine()
    {
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("Im in the end room");
    }
}
