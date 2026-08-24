using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPossession : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerBody;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private MonoBehaviour playerMovement;

    [Header("Object Camera")]
    [SerializeField] private string objectCameraTag = "ObjectCamera";

    private HoverOutline hoverOutline;

    private Camera objectCamera;
    private GameObject possessedObject;

    private bool isPossessing = false;

    private void Awake()
    {
        hoverOutline = GetComponent<HoverOutline>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        //enter possession
        if (!isPossessing && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryPossess();
        }

        //exit possession
        if (isPossessing && Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            ExitPossession();
        }
    }

    private void TryPossess()
    {
        if (hoverOutline == null)
            return;

        Outline hoveredOutline = hoverOutline.GetCurrentOutline();

        if (hoveredOutline == null)
            return;

        //object with the Outline component
        possessedObject = hoveredOutline.gameObject;

        //look for camera on the object or its children
        objectCamera = possessedObject.GetComponentInChildren<Camera>(true);

        if (objectCamera == null)
        {
            Debug.LogWarning(
                $"No camera found on {possessedObject.name} or its children."
            );

            possessedObject = null;
            return;
        }

        //disable player
        if (playerBody != null)
            playerBody.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);

        //enable object camera
        objectCamera.gameObject.SetActive(true);

        isPossessing = true;
    }

    private void ExitPossession()
    {
        if (!isPossessing)
            return;

        //disable object camera
        if (objectCamera != null)
            objectCamera.gameObject.SetActive(false);

        //give player control back
        if (playerBody != null)
            playerBody.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        possessedObject = null;
        objectCamera = null;

        isPossessing = false;
    }

    private void OnDisable()
    {
        if (isPossessing)
        {
            ExitPossession();
        }
    }
}