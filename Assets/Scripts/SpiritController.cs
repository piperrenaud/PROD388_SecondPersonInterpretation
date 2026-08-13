using UnityEngine;
using UnityEngine.InputSystem;

public class SpiritController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float collisionRadius = 0.4f;
    [SerializeField] private float collisionHeight = 1.8f;
    [SerializeField] private float collisionSkin = 0.05f;

    [Header("Camera")]
    [SerializeField] private Camera playerCam;
    [SerializeField] private float cameraDistance = 6f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private float minLookAngle = -30f;
    [SerializeField] private float cameraCollisionRadius = 0.2f;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;

    private float pitch;
    private float yaw;
    private bool movementEnabled = true;

    private void Start()
    {
        Vector3 startingAngles = transform.eulerAngles;

        yaw = startingAngles.y;
        pitch = 15f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateCamera();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
    }

    private void Update()
    {
        if (!movementEnabled) return;

        HandleLook();
        HandleMovement();
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    private void HandleLook()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;

        pitch = Mathf.Clamp(
            pitch,
            minLookAngle,
            maxLookAngle);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        //camera relative movement
        Vector3 forward = playerCam.transform.forward;
        Vector3 right = playerCam.transform.right;  

        Vector3 movement = forward * moveInput.y + right * moveInput.x;

        //dialag limits
        if (movement.sqrMagnitude > 1f) movement.Normalize();
        
        Vector3 displacement = movement * moveSpeed * Time.deltaTime;
        MoveWithCollision(displacement);
    }

    private void MoveWithCollision(Vector3 displacment)
    {
        if (displacment.sqrMagnitude <= 0f) return;

        Vector3 center = transform.position;
        float radius = collisionRadius;

        //botton and top of spirit
        Vector3 bottom = center + Vector3.down * (collisionHeight * 0.5f - radius);
        Vector3 top = center + Vector3.up * (collisionHeight * 0.5f - radius);

        float distance = displacment.magnitude;

        if (Physics.CapsuleCast(
            bottom, 
            top,
            radius,
            displacment.normalized,
            out RaycastHit hit,
            distance + collisionSkin,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            float allowedDistance = Mathf.Max(hit.distance - collisionSkin, 0f);

            transform.position += displacment.normalized * allowedDistance;
            return;
        }

        transform.position += displacment;
    }

    private void UpdateCamera()
    {
        Vector3 targetPosition = transform.position + Vector3.up * cameraHeight;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = targetPosition - rotation * Vector3.forward * cameraDistance;

        //stop camera form going through walls
        Vector3 direction = desiredPosition - targetPosition;

        float distance = direction.magnitude;

        if (Physics.SphereCast(
            targetPosition,
            cameraCollisionRadius,
            direction.normalized,
            out RaycastHit hit,
            distance,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            desiredPosition = targetPosition + direction.normalized * Mathf.Max(hit.distance - cameraCollisionRadius, 0f);
        }

        playerCam.transform.position = desiredPosition;
        playerCam.transform.rotation = rotation;
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }
}
