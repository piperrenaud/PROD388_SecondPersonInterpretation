using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Interact : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask blockingLayers;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    private Camera playerCamera;
    private Outline outline;
    private OpenDoor door;
    private bool isHovered;

    private void Start()
    {
        playerCamera = Camera.main;
        outline = GetComponent<Outline>();

        door = GetComponentInParent<OpenDoor>();

        if (outline != null)
        {
            outline.enabled = false;
        }

        if (door == null) Debug.LogError("No door in parent");
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
        if (Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);

        bool newHoverState = false;

        //everything ray hits, closest to furtheest
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            //if hit a wall/obstacle first, stop checking
            if (((1 << hit.transform.gameObject.layer) & blockingLayers) != 0)
            {
                break;
            }

            OpenDoor hitDoor = hit.transform.GetComponentInParent<OpenDoor>();

            if (hitDoor == door)
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
        if (dialogueManager != null && dialogueManager.IsDialogueRunning) return;
        if (!isHovered) return;

        OpenDoor door = GetComponent<OpenDoor>();

        if (door != null)
        {
            door.Interact();
        }
    }
}
