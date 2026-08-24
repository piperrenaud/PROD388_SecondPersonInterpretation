using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    [SerializeField] private NavMeshLink navMeshLink;
    [SerializeField] private bool isOpen = false;

    [HideInInspector] public bool IsOpen => isOpen;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        UpdateNavMeshLink();
    }

    public void Open()
    {
        isOpen = true;
        animator.SetTrigger("Open");
        UpdateNavMeshLink();
    }

    public void Close()
    {
        isOpen = false;
        animator.SetTrigger("Close");
        UpdateNavMeshLink();
    }

    private void UpdateNavMeshLink()
    {
        if (navMeshLink != null)
        {
            navMeshLink.enabled = isOpen;
        }
    }
}
