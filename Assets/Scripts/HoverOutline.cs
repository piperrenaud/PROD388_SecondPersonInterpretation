using UnityEngine;
using UnityEngine.InputSystem;

public class HoverOutline : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera playerCam;
    [SerializeField] private float maxDistance = 100f;

    private Outline currentOutline;

    private void Awake()
    {
        if (playerCam == null) playerCam = Camera.main;
    }

    private void Update()
    {
        CheckMouseHover();
    }

    private void CheckMouseHover()
    {
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

    public Outline GetCurrentOutline()
    {
        return currentOutline;
    }
}
