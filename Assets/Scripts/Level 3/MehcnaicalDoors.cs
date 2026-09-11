using Unity.AI.Navigation;
using UnityEngine;

public class MehcnaicalDoors : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private Room roomA;
    [SerializeField] private Room roomB;

    [Header("Navigation")]
    [SerializeField] private Transform roomATarget;
    [SerializeField] private Transform roomBTarget;
    [SerializeField] private NavMeshLink navMeshLink;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Locked?")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool isDoor3 = false;
    [SerializeField] private bool isDoor5 = false;
    [SerializeField] private string requiredItem;
    [SerializeField] private string lockedDoorText = "Hmm this door is locked.";
    [SerializeField] private Room beforeLockedRoom;
    [SerializeField] private ObjectiveArea doorsObjective;

    [Header("Character")]
    [SerializeField] private CharacterDoorController character;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Audio")]
    [SerializeField] private AudioClip openAudio;
    [SerializeField] private AudioSource audioSource;

    public Room RoomA => roomA;
    public Room RoomB => roomB;

    public bool IsOpen { get; private set; }
    public bool IsLocked => isLocked;

    private Inventory inventory;

    private void Start()
    {
        inventory = character.GetComponent<Inventory>();
    }
    private void Update()
    {
        if (inventory != null)
        {
            if (inventory.HasItem(requiredItem))
            {
                isLocked = false;
            }
        }
    }

    public void OpenDoor()
    {
        if (IsOpen) return;

        //Locked door
        if (isLocked)
        {
            if ((isDoor3 || isDoor5) && character != null && character.CurrentRoom == beforeLockedRoom)
            {
                character.GoToLockedDoor(this);
            }

            return;
        }

        IsOpen = true;

        if (navMeshLink != null )
        {
            navMeshLink.enabled = true;
        }

        animator.SetTrigger("Open");
        audioSource.PlayOneShot(openAudio);

        DoorManager.Instance.SetOpenDoor(this);
    }

    public void CloseDoor(bool notifyManager = true)
    {
        if (!IsOpen) return;

        IsOpen = false;

        if ( navMeshLink != null )
        {
            navMeshLink.enabled = false;
        }

        animator.SetTrigger("Close");

        if (notifyManager)
        {
            DoorManager.Instance.ClearOpenDoor(this);
        }
    }

    public bool ConnectsToRoom(Room room)
    {
        return room == roomA || room == roomB;
    }

    public Room GetOtherRoom(Room currentRoom)
    {
        if (currentRoom == roomA) return roomB;
        if (currentRoom == roomB) return roomA;

        return null;
    }

    public Transform GetTargetFromRoom(Room currentRoom)
    {
        if (currentRoom == roomA) return roomBTarget;
        if (currentRoom == roomB) return roomATarget;

        return null;
    }

    public void ToggleDoor()
    {
        if (IsOpen) CloseDoor();
        else OpenDoor();
    }

    public void OnLockedDoorReached()
    {
        if (doorsObjective != null && doorsObjective.ObjectiveActive)
            return;

        //shelf objective is active, don't want door dialogue being triggered over it
        if (isDoor3 && ObjectiveManager.Instance.CurrentObjectiveIndex == 2) return;

        dialogueManager.SetText(lockedDoorText);
    }

    public Transform GetLockedDoorTarget()
    {
        return roomATarget;
    }
}
