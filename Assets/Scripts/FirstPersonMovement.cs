using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;

    [Header("Mouse Look")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundedGravity = -2f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepSounds;
    [Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;
    [SerializeField] private float footstepInterval = 0.4f;

    private CharacterController controller;
    private AudioSource source;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float cameraPitch;
    private float verticalVelocity;
    private float stepTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        source = GetComponentInChildren<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleAnimation();
        HandleFootsteps();
    }

    private void HandleMovement()
    {
        Vector3 movement = transform.right * moveInput.x +
                            transform.forward * moveInput.y;
        movement *= moveSpeed;

        //gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = groundedGravity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        //horizontal
        float mouseX = lookInput.x * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        //vertical
        float mouseY = lookInput.y * mouseSensitivity;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(
            cameraPitch,
            -maxLookAngle,
            maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(
            cameraPitch,
            0f,
            0f);
    }

    private void HandleAnimation()
    {
        bool isRunning = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("IsRunning", isRunning);
    }

    private void HandleFootsteps()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        bool isGrounded = controller.isGrounded;

        //no footsteps while still or in air
        if (!isMoving || !isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer += Time.deltaTime;

        if (stepTimer >= footstepInterval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0 || source == null) return;

        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];

        if (clip == null) return;

        source.pitch = Random.Range(minPitch, maxPitch);
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 1.5f;
        source.maxDistance = 12f;
        source.dopplerLevel = 0f;

        source.PlayOneShot(clip, volume);
    }

    public void Footstep()
    {
        HandleFootsteps();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
