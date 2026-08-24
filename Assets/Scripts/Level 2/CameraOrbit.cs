using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Orbit")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float cameraRadius = 0.25f;
    [SerializeField] private float wallOffset = 0.1f;

    private float yaw;
    private float pitch = 20f;
    private float previousTargetYaw;

    private void Start()
    {
        if (target == null) return;

        //start camera behind target
        yaw = target.eulerAngles.y;
        previousTargetYaw = target.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        CheckForTargetRotation();

        HandleInput();
        UpdateCameraPosition();
    }

    private void CheckForTargetRotation()
    {
        float currentTargetYaw = target.eulerAngles.y;

        //rotation since last frame
        float angleDifference = Mathf.DeltaAngle(previousTargetYaw, currentTargetYaw);

        //if target rotated, camera rotates
        if (Mathf.Abs(angleDifference) > 0.1f)
        {
            yaw = currentTargetYaw;
        }

        previousTargetYaw = currentTargetYaw;
    }

    private void HandleInput()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null) return;

        //hold right mouse button to orbit
        if (mouse.rightButton.isPressed)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();

            yaw += mouseDelta.x * rotationSpeed;
            pitch -= mouseDelta.y * rotationSpeed;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        //scroll to zoom
        float scroll = mouse.scroll.ReadValue().y;

        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        
        //where the camera wants to be
        Vector3 direction = rotation * Vector3.back;
        Vector3 desiredPosition = target.position + direction * distance;

        float finalDistance = distance;

        //check if theres a wall in the way
        if (Physics.SphereCast(
            target.position,
            cameraRadius,
            direction,
            out RaycastHit hit,
            distance,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            finalDistance = hit.distance - wallOffset;

            //stop camera from going trhough target
            finalDistance = Mathf.Max(finalDistance, 0.1f);
        }

        transform.position = target.position + direction * finalDistance;
        transform.LookAt(target.position);
    }
}
