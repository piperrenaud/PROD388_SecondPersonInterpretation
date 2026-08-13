using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ProtagonistPathFinder : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private Transform endPoint;

    [Header("Movemnet")]
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Path Refresh")]
    [SerializeField] private float retryInterval = 0.25f;

    private NavMeshAgent agent;
    private float retryTimer;

    [HideInInspector] public Transform EndPoint => endPoint;
    [HideInInspector] public bool HasReachedDestination { get; private set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;

        agent.autoTraverseOffMeshLink = true;
    }

    private void Start()
    {
        if (endPoint != null)
        {
            SetDestination(endPoint);
        }
    }

    private void Update()
    {
        if (!agent.isOnNavMesh) return;
        if (endPoint == null) return;
        if (HasReachedDestination) return;
        if (agent.isOnOffMeshLink) return;
        if (agent.pathPending) return;

        //try find a complete path
        if (agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Reached Destination");
            HasReachedDestination = true;
            agent.isStopped = true;
            return;
        }

        //has partial path
        if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            if (!agent.hasPath)
            {
                retryTimer -=Time.deltaTime;

                if (retryTimer <= 0f)
                {
                    retryTimer = retryInterval;
                    TryFindPath();
                }
            }

            return;
        }

        //no valid complete path? keep trying
        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            retryTimer -= Time.deltaTime;

            if (retryTimer <= 0f)
            {
                retryTimer = retryInterval;
                TryFindPath();
            }
        }
    }

    public void SetDestination(Transform newEndPoint)
    {
        if (newEndPoint == null)
        {
            Debug.LogWarning($"{name}: EndPoint is null");
            return;
        }

        endPoint = newEndPoint;
        HasReachedDestination = false;

        retryTimer = 0f;
        TryFindPath();
    }

    private void TryFindPath()
    {
        if (endPoint == null) return;
        if (!agent.isOnNavMesh) return;
        if (agent.isOnOffMeshLink) return;

        NavMeshPath path = new NavMeshPath();

        bool found = NavMesh.CalculatePath(
            agent.nextPosition,
            endPoint.position,
            NavMesh.AllAreas,
            path);

        Debug.Log($"{name}: " + $"Found={found}, " + $"Status={path.status}");

        if (found && 
            path.status == NavMeshPathStatus.PathComplete ||
            path.status == NavMeshPathStatus.PathPartial)
        {
            agent.isStopped = false;
            agent.SetPath(path);

            HasReachedDestination = false;
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();

            HasReachedDestination = false;
        }
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.ResetPath();
        HasReachedDestination = false;
    }

    public void Resume()
    {
        if (endPoint == null) return;

        HasReachedDestination = false;
        retryTimer = 0f;

        TryFindPath();
    }
}
