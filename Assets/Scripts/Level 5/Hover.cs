using UnityEngine;
using UnityEngine.InputSystem;

public class Hover : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera playerCam;
    [SerializeField] private float maxDistance = 100f;

    private Outline currentOutline;

    private bool hasKey = false;

    private void Awake()
    {
        if (playerCam == null) playerCam = Camera.main;

        currentOutline = null;
    }

    private void Update()
    {
        CheckMouseHover();
        CheckInteraction();
    }

    private void CheckMouseHover()
    {
        if (Mouse.current == null || playerCam == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = playerCam.ScreenPointToRay(mousePosition);

        Outline newOutline = null;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            newOutline = hit.collider.GetComponentInParent<Outline>();
        }

        if (newOutline == currentOutline) return;

        if (currentOutline != null) currentOutline.enabled = false;
        if (newOutline != null) newOutline.enabled = true;

        currentOutline = newOutline;
    }

    private void OnDisable()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    private void CheckInteraction()
    {
        if (currentOutline == null) return; 
        if (Keyboard.current == null) return;
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        OpenDoor door = currentOutline.GetComponentInParent<OpenDoor>();
        if (door != null)
        {
            if (!hasKey) door.Interact();
            else door.LockedEndDoor();
        }

        LeverSwitch lever = currentOutline.GetComponentInParent<LeverSwitch>();
        if (lever != null)
        {
            lever.ToggleLever();
        }

        Key key = currentOutline.GetComponentInParent<Key>();
        if (key != null)
        {
            key.PickUp();
            hasKey = true;
        }
    }

    public Outline GetCurrentOutline()
    {
        return currentOutline;
    }
}
