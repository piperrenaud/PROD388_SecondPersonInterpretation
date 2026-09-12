using UnityEngine;
using UnityEngine.AI;

public class ProtagonistWander : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform wanderCentre;
    [SerializeField] private IntroAndEnd introAndEnd;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;
    [SerializeField] private float footstepInterval = 0.4f;

    [Header("Wandering")]
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minWanderDistance = 2f;
    [SerializeField] private float wanderWaitTime = 1f;

    [Header("Escape")]
    [SerializeField] private float pathCheckInterval = 0.25f;

    private float wanderTimer;
    private float pathCheckTimer;

    private bool isEscaping = false;

    private float footstepTimer;
    private AudioSource source;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PickNewWanderPoint();
    }

    private void Update()
    {
        if (!isEscaping)
        {
            pathCheckTimer -= Time.deltaTime;

            if (pathCheckTimer <= 0f)
            {
                pathCheckTimer = pathCheckInterval;

                if (CanReachExit())
                {
                    StartEscaping();
                    return;
                }
            }
        }

        //is escaping let navmesh agent handle movment
        if (isEscaping)
        {
            HandleEscape();
        }
        else
        {
            HandleWandering();
        }

        UpdateAnimation();
        HandleFootsteps();
    }

    private void HandleWandering()
    {
        if (agent.pathPending) return;

        //wait to reach current wadner point
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0f)
            {
                PickNewWanderPoint();
            }
        }
    }

    private void PickNewWanderPoint()
    {
        if (wanderCentre == null) return;

        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;

        randomDir.y = 0f;

        Vector3 randomPoint = wanderCentre.position + randomDir;

        if (NavMesh.SamplePosition(
            randomPoint,
            out NavMeshHit hit,
            wanderRadius,
            NavMesh.AllAreas))
        {
            //dont pick point too close to current
            if (Vector3.Distance(transform.position, hit.position) >= minWanderDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                wanderTimer = wanderWaitTime;
            }
        }
    }

    private bool CanReachExit()
    {
        if (exitPoint == null) return false;
        if (!agent.isOnNavMesh) return false;

        if (!NavMesh.SamplePosition(
            exitPoint.position,
            out NavMeshHit exitHit,
            1f,
            NavMesh.AllAreas))
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();

        bool foundPath = agent.CalculatePath(exitHit.position, path);

        Debug.Log($"Exit path: {path.status}");

        if (!foundPath) return false;

        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void StartEscaping()
    {
        if (isEscaping) return;

        isEscaping = true;
        agent.isStopped = false;

        if (NavMesh.SamplePosition(
            exitPoint.position,
            out NavMeshHit exitHit,
            1f,
            NavMesh.AllAreas))
        {
            agent.SetDestination(exitHit.position);
        }

        Debug.Log("Found path to exit!");
    }

    private void HandleEscape()
    {
        if (agent.pathPending) return;

        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            if (NavMesh.SamplePosition(
                exitPoint.position,
                out NavMeshHit exithit,
                1f,
                NavMesh.AllAreas))
            {
                agent.SetDestination(exitPoint.position);
            }

            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            ReachedExit();
        }
    }

    private void ReachedExit()
    {
        agent.isStopped = true;
        animator.SetBool("IsRunning", false);

        introAndEnd.EndLevel();
    }

    private void UpdateAnimation()
    {
        if (animator == null || agent == null) return;

        bool isMoving = !agent.isStopped && agent.velocity.sqrMagnitude > 0.01f;

        animator.SetBool("IsRunning", isMoving);
    }

    private void PlayFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0 || source == null) return;

        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];

        if (clip == null) return;

        source.pitch = Random.Range(minPitch, maxPitch);

        source.PlayOneShot(clip, volume);
    }

    private void HandleFootsteps()
    {
        bool isMoving = agent.velocity.sqrMagnitude > 0.01f;

        if (!isMoving)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            PlayFootstep();
            footstepTimer = footstepInterval;
        }
    }
}
