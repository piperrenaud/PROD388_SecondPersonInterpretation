using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPossession : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private MonoBehaviour playerMovement;

    [Header("Object Camera")]
    [SerializeField] private string objectCameraTag = "ObjectCamera";

    [Header("Possessed Object Movement")]
    [SerializeField] private float objectMoveSpeed = 5f;

    [Header("Dragging Audio")]
    [SerializeField] private AudioSource draggingAudioSource;
    [SerializeField] private AudioClip[] draggingClip;

    private HoverOutline hoverOutline;

    private Camera objectCamera;
    private GameObject possessedObject;

    private bool isPossessing = false;

    private void Awake()
    {
        hoverOutline = GetComponent<HoverOutline>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (draggingAudioSource != null)
        {
            draggingAudioSource.loop = true;
            draggingAudioSource.playOnAwake = false;
        }
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

        //while possessing
        if (isPossessing)
        {
            HandleObjectMovement();

            //exit possession
            if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
            {
                ExitPossession();
            }
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

        //disable player object movement/camera
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);

        //enable object camera
        objectCamera.gameObject.SetActive(true);

        possessedObject.GetComponent<Rigidbody>().isKinematic = false;

        isPossessing = true;
    }

    private void HandleObjectMovement()
    {
        if (possessedObject == null || objectCamera == null)
        {
            StopDraggingSound();
            return;
        }

        Vector2 input = Vector2.zero;

        //WASD
        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;

        //stop diag movement from being faster
        if (input.sqrMagnitude > 1f) input.Normalize();

        bool isMoving = input.sqrMagnitude > 0f;

        if (!isMoving)
        {
            StopDraggingSound();
            return;
        }

        //camera dir
        Vector3 forward = objectCamera.transform.forward;
        Vector3 right = objectCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        //camera relative movement
        Vector3 movement = forward * input.y + right * input.x;

        possessedObject.transform.position += movement * objectMoveSpeed * Time.deltaTime;

        StartDraggingSound();
    }

    private void StartDraggingSound()
    {
        if (draggingAudioSource == null) return;
        if (draggingAudioSource.isPlaying) return;
        if (draggingClip == null || draggingClip.Length == 0) return;

        AudioClip clip = draggingClip[Random.Range(0, draggingClip.Length)];

        draggingAudioSource.clip = clip;
        draggingAudioSource.loop = true;
        draggingAudioSource.Play();
    }

    private void StopDraggingSound()
    {
        if (draggingAudioSource == null) return;

        if (draggingAudioSource.isPlaying)
        {
            draggingAudioSource.Stop();
        }
    }

    private void ExitPossession()
    {
        if (!isPossessing)
            return;

        StopDraggingSound();

        //disable object camera
        if (objectCamera != null)
            objectCamera.gameObject.SetActive(false);

        possessedObject.GetComponent<Rigidbody>().isKinematic = true;

        //enable player object movement/camera
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