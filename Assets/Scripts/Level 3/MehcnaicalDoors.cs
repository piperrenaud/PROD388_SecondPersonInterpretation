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

    public Room RoomA => roomA;
    public Room RoomB => roomB;

    public bool IsOpen { get; private set; }

    public void OpenDoor()
    {
        if (IsOpen) return;

        IsOpen = true;

        if (navMeshLink != null )
        {
            navMeshLink.enabled = true;
        }

        animator.SetTrigger("Open");

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
}
