using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float maxDistance = 100f;

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

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                newHoverState = true;
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
