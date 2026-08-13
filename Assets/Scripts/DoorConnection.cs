using UnityEngine;

public class DoorConnection : MonoBehaviour
{
    [Header("Cell this door connects")]
    [SerializeField] private Vector2Int cellA;
    [SerializeField] private Vector2Int cellB;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

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
