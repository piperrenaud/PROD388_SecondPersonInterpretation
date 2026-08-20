using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask blockingLayers;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Central Interaction Manager")]
    [SerializeField] private LevelOneInteractions levelInteractions;

    private Camera playerCamera;
    private Outline outline;

    private bool isHovered;

    private void Start()
    {
        playerCamera = Camera.main;
        outline = GetComponent<Outline>();

        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
    }

    private void Update()
    {
        if (playerCamera == null) return;

        CheckHover();
    }

    private void CheckHover()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);

        bool newHoverState = false;

        //everything ray hits, closest to furtheest
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
        //closest to furthest
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            //if hit a wall/obstacle first, stop checking
            if (((1 << hit.transform.gameObject.layer) & blockingLayers) != 0)
            {
                break;
            }

            //if hit belongs to this interactable
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                newHoverState = true;
                break;
            }
        }

        if (newHoverState != isHovered)
        {
            isHovered = newHoverState;

            if (outline != null)
            {
                outline.enabled = isHovered;
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isHovered)
        {
            return;
        }

        if (levelInteractions != null)
        {
            levelInteractions.HandleInteraction(gameObject);
        }
    }
}
