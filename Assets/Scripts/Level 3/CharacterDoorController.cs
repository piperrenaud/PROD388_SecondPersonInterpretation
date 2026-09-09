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
    private AudioSource source;
    private Vector3 targetPosition;

    private float footstepTimer;

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

        bool isMoving = agent.velocity.sqrMagnitude > 0.01f;

        animator.SetBool("IsRunning", isMoving);

        //footsteps
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

        //reached target point
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

        Debug.Log("Character entered: " + currentRoom.roomName);
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
}
