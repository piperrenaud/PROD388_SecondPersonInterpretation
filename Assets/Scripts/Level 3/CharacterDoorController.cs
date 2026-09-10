using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CharacterDoorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Current State")]
    [SerializeField] private Room currentRoom;

    [Header("Room Detection")]
    [SerializeField] private float roomDetectionDistance = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;
    [SerializeField] private float footstepInterval = 0.4f;

    private MehcnaicalDoors targetDoor;
    private Room targetRoom;

    private Vector3 targetPosition;

    private KeyObject currentObjectiveTarget;
    private LockedCabinet currentCabinetTarget;

    private float footstepTimer;
    private AudioSource source;

    public Room CurrentRoom => currentRoom;

    private void Start()
    {
        agent.isStopped = true;
        animator.SetBool("IsRunning", false);

        source = GetComponent<AudioSource>();

        if (source != null)
        {
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 1.5f;
            source.maxDistance = 12f;
            source.dopplerLevel = 0f;
        }
    }

    public void CheckOpenDoor()
    {
        if (DoorManager.Instance == null) return;

        MehcnaicalDoors openDoor = DoorManager.Instance.CurrentOpenDoor;

        //no doors open
        if (openDoor == null)
        {
            targetDoor = null;
            targetRoom = null;

            StopMoving();
            return;
        }

        //door doesnt connect to current room
        if (!openDoor.ConnectsToRoom(currentRoom))
        {
            targetDoor = null;
            targetRoom = null;

            StopMoving();
            return;
        }

        //character already moving through this door
        if (targetDoor == openDoor)
        {
            return;
        }

        //set new target
        targetDoor = openDoor;
        targetRoom = openDoor.GetOtherRoom(currentRoom);

        Transform destination = openDoor.GetTargetFromRoom(currentRoom);

        if (destination != null)
        {
            targetPosition = destination.position; 

            agent.isStopped = false;
            agent.SetDestination(targetPosition);

            animator.SetBool("IsRunning", true);
        }
    }

    private void Update()
    {
        //going to a KeyObject
        if (currentObjectiveTarget != null)
        {
            UpdateObjectiveTarget();
            return;
        }

        //going to a locked cabinet
        if (currentCabinetTarget != null)
        {
            UpdateCabinetTarget();
            return;
        }
        
        //not travelling through a door
        if (targetDoor == null)
        {
            animator.SetBool("IsRunning", false);
            footstepTimer = 0f;
            return;
        }

        //door closed before char reach it
        if (!targetDoor.IsOpen)
        {
            targetDoor = null;
            targetRoom = null;

            StopMoving();
            return;
        }

        if (agent.pathPending) return;

        UpdateMovementAnimation();

        float distanceToTarget = Vector3.Distance(
            transform.position, targetPosition);

        if (distanceToTarget <= roomDetectionDistance)
        {
            EnterNextRoom();
        }
    }

    private void EnterNextRoom() 
    {
        currentRoom = targetRoom; 

        targetDoor = null; 
        targetRoom = null; 

        StopMoving();

        //check if current objective has target in room
        CheckForObjectiveTarget();
    }

    private void CheckForObjectiveTarget()
    {
        if (ObjectiveManager.Instance == null) return;
        if (!ObjectiveManager.Instance.ObjectiveActive) return;

        //check locked cabinet
        LockedCabinet cabinet = GetObjectiveCabinet(currentRoom);

        if (cabinet != null)
        {
            GoToCabinet(cabinet);
            return;
        }
        
        // check KeyObjects
        KeyObject keyObject = GetObjectiveTarget(currentRoom);

        if (keyObject != null)
        {
            GoToObjectiveTarget(keyObject);
            return;
        }

        
    }

    private void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();

        animator.SetBool("IsRunning", false);

        footstepTimer = 0f;
    }

    private void PlayFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0 || source == null) return;

        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];

        if (clip == null) return;

        source.pitch = Random.Range(minPitch, maxPitch);

        source.PlayOneShot(clip, volume);
    }

    private KeyObject GetObjectiveTarget(Room room)
    {
        KeyObject[] objects = FindObjectsByType<KeyObject>(FindObjectsSortMode.None);

        foreach (KeyObject keyObject in objects)
        {
            if (keyObject.Room != room) continue;
            if (!keyObject.IsCurrentObjective()) continue;

            return keyObject;
        }

        return null;
    }

    private LockedCabinet GetObjectiveCabinet(Room room)
    {
        if (ObjectiveManager.Instance == null) return null;
        if (!ObjectiveManager.Instance.ObjectiveActive) return null;

        int currentObjective = ObjectiveManager.Instance.CurrentObjectiveIndex;

        LockedCabinet[] cabinets = FindObjectsByType<LockedCabinet>(FindObjectsSortMode.None);

        foreach (LockedCabinet cabinet in cabinets)
        {
            if (cabinet.Room != room) continue;

            if (cabinet.ObjectiveOrder != currentObjective) continue;

            if (!cabinet.IsCurrentObjective()) continue;

            return cabinet;
        }
        
        return null;
    }

    private void GoToObjectiveTarget(KeyObject objectiveTarget)
    {
        if (objectiveTarget.TargetPoint == null) return;

        currentObjectiveTarget = objectiveTarget;

        targetPosition = objectiveTarget.TargetPoint.position;

        agent.isStopped = false;
        agent.SetDestination(targetPosition);

        animator.SetBool("IsRunning", true);
    }

    private void UpdateObjectiveTarget()
    {
        if (currentObjectiveTarget == null) return;

        //objective changed while travellign
        if (!currentObjectiveTarget.IsCurrentObjective())
        {
            currentObjectiveTarget = null;
            StopMoving();
            return;
        }

        if (agent.pathPending) return;

        UpdateMovementAnimation();

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget <= roomDetectionDistance)
        {
            ReachObjectiveTarget();
        }
    }

    private void ReachObjectiveTarget()
    {
        if (currentObjectiveTarget == null) return;

        StopMoving();

        KeyObject target = currentObjectiveTarget;

        currentObjectiveTarget = null;

        target.ReachTarget();

        //objective mightve changed, check if new objective has target in current room
        CheckForObjectiveTarget();
    }

    private void GoToCabinet(LockedCabinet cabinet)
    {
        if (cabinet == null) return;

        if (cabinet.TargetPoint == null) return;

        currentCabinetTarget = cabinet;
        targetPosition = cabinet.TargetPoint.position;

        agent.isStopped = false;
        agent.SetDestination(targetPosition);

        animator.SetBool("IsRunning", true);
    }

    private void UpdateCabinetTarget()
    {
        if (currentCabinetTarget == null) return;
        if (agent.pathPending) return;

        UpdateMovementAnimation();

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget <= roomDetectionDistance)
        {
            ReachCabinet();
        }
    }

    private void ReachCabinet()
    {
        if (currentCabinetTarget == null) return;

        StopMoving();

        LockedCabinet cabinet = currentCabinetTarget;

        currentCabinetTarget = null;

        //actually interact with cabinet
        cabinet.TryOpen(GetComponent<CharacterInventory>(), currentRoom);
    }

    private void UpdateMovementAnimation()
    {
        bool isMoving = agent.velocity.sqrMagnitude > 0.01f;

        animator.SetBool("IsRunning", isMoving);

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
}
