using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LevelOneInteractions levelOneInteractions;

    [Header("Animation")]
    [SerializeField] private string walkingBool = "IsWalking";

    private Transform currentTarget;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    public void WalktTo(Vector3 destination, Transform lookTarget)
    {
        if (agent == null)
        {
            Debug.LogWarning(name + " has no navmeshagent");
            return;
        }

        currentTarget = lookTarget;

        agent.isStopped = false;
        agent.SetDestination(destination);

        animator.SetBool(walkingBool, true);
    }

    public void StopWalking()
    {
        if (agent == null) return;

        agent.isStopped = true;
        animator.SetBool(walkingBool, false);

        animator.SetTrigger("Scared");
    }

    private void Update()
    {
        if (agent == null) return;
        if (!levelOneInteractions.IsTimerUp) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StopWalking();

            //face destination
            if (currentTarget != null)
            {
                Vector3 direction = currentTarget.position - transform.position;

                direction.y = 0f;

                if (direction.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }

    public void LookAt(Transform target)
    {
        if(target == null) return;

        Vector3 direction = target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
