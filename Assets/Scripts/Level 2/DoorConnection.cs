using UnityEngine;

public class DoorConnection : MonoBehaviour
{
    [Header("Cell this door connects")]
    [SerializeField] private Vector2Int cellA;
    [SerializeField] private Vector2Int cellB;

    [Header("Blocked Door?")]
    [SerializeField] private bool isBlocked = false;
    [TextArea(2, 4)]
    [SerializeField] private string blockedDialogue = "That way is blocked.";

    [SerializeField] private Animator animator;

    public bool IsBlocked => isBlocked;
    public string BlockedDialogue => blockedDialogue;

    public void OpenDoor()
    {
        animator.SetTrigger("Open");
    }

    public void CloseDoor()
    {
        animator.SetTrigger("Close");
    }

    public bool Connects(Vector2Int from, Vector2Int to)
    {
        return (cellA == from && cellB == to) ||
            (cellA == to && cellB == from);
    }
}
