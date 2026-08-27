using System.Collections;
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

    [Header("Narrator")]
    [SerializeField] private NarratorManager narratorManager;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Movement Narration")]
    [SerializeField] private string idleEventOne = "PlayerStandingStillOne";
    [SerializeField] private string idleEventTwo = "PlayerStandingStillTwo";
    [SerializeField] private string idleEventThree = "PlayerStandingStillThree";
    [SerializeField] private string idleEventFour = "PlayerStandingStillRespawn";
    [SerializeField] private float idleTimeBetweenEvents = 10f;

    [Header("Mouse Narration")]
    [SerializeField] private string erraticMouseEventOne = "ErraticMouseMovementOne";
    [SerializeField] private string erraticMouseEventTwo = "ErraticMouseMovementTwo";
    [SerializeField] private string erraticMouseEventRespawn = "ErraticMouseMovementRespawn";
    [SerializeField] private string postRespawnEvent = "SecondRespawn";
    [SerializeField] private float erraticCheckWindow = 4f;
    [SerializeField] private float erraticMovementThreshold = 10000f;
    [SerializeField] private int erraticDirectionChangesRequired = 7;
    [SerializeField] private float erraticResetTime = 5f;

    private CharacterController controller;
    private AudioSource source;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float cameraPitch;
    private float verticalVelocity;
    private float stepTimer;

    private float idleTimer = 0f;
    private int idleStage = 0;
    private bool waitingForDialogue = false;

    private float erraticTimer = 0f;
    private float erraticMovementAmount = 0f;
    private int directionChanges = 0;
    private float previousMouseX = 0f;
    private bool hasPreviousMouseDirection = false;
    private int erraticStage = 0;
    private float totalErraticMovement = 0f;
    private float timeSinceErraticMovement = 0f;
    private bool hasCausedErraticRespawn = false;

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
        HandleIdleNarration();
        HandleErraticMouse();
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

    private void HandleIdleNarration()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        bool isLooking = lookInput.sqrMagnitude > 0.01f;

        if (isMoving || isLooking)
        {
            ResetIdleSequence();
            return;
        }

        if (waitingForDialogue) return;

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTimeBetweenEvents)
        {
            StartCoroutine(TriggerNextIdleEvent());
        }
    }

    private IEnumerator TriggerNextIdleEvent()
    {
        waitingForDialogue = true;
        idleTimer = 0f;
        string eventID = null;

        switch (idleStage)
        {
            case 0:
                eventID = idleEventOne;
                break;

            case 1:
                eventID = idleEventTwo;
                break;

            case 2:
                eventID = idleEventThree;
                break;

            case 3:
                eventID = idleEventFour;
                break;

            default:
                waitingForDialogue = false;
                yield break;
        }

        if (narratorManager != null && !string.IsNullOrEmpty(eventID))
        {
            narratorManager.TriggerEvent(eventID);
            idleStage++;
        }

        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        idleTimer = 0f;
        waitingForDialogue = false;
    }

    private void ResetIdleSequence()
    {
        idleTimer = 0f;
        idleStage = 0;
        waitingForDialogue = false;
    }

    private void HandleErraticMouse()
    {
        timeSinceErraticMovement += Time.deltaTime;

        if (timeSinceErraticMovement >= erraticResetTime)
        {
            ResetErraticSequence();
            return;
        }

        if (dialogueManager != null && dialogueManager.IsDialogueRunning)
        {
            return;
        }

        float mouseX = lookInput.x;

        //mouse movements>
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            float movement = Mathf.Abs(mouseX);

            erraticMovementAmount += movement;
            totalErraticMovement += movement;

            //change in horizontal direction?
            float currentDirection = Mathf.Sign(mouseX);

            if (hasPreviousMouseDirection && currentDirection != previousMouseX)
            {
                directionChanges++;
            }

            previousMouseX = currentDirection;
            hasPreviousMouseDirection = true;
        }

        erraticTimer += Time.deltaTime;

        //player moving mouse crazy in time period?
        if (erraticTimer >= erraticCheckWindow)
        {
            Debug.Log(
                "Mouse movement: " + erraticMovementAmount +
                " | Direction changes: " + directionChanges
            );

            bool isErratic = erraticMovementAmount >= erraticMovementThreshold &&
                directionChanges >= erraticDirectionChangesRequired;

            if ( isErratic )
            {
                CheckErraticStage();
            }

            //reset measurement
            erraticTimer = 0f;
            erraticMovementAmount = 0f;
            directionChanges = 0;
            hasPreviousMouseDirection = false;
        }
    }

    private void CheckErraticStage()
    {
        if (hasCausedErraticRespawn)
        {
            if (totalErraticMovement >= erraticMovementThreshold)
            {
                TriggerErraticMouseEvent(postRespawnEvent);
                erraticStage = 3;
            }
            
            return;
        }

        switch ( erraticStage )
        {
            case 0:
                if (totalErraticMovement >= erraticMovementThreshold)
                {
                    TriggerErraticMouseEvent(erraticMouseEventOne);
                    erraticStage = 1;
                }
                break;

            case 1:
                if (totalErraticMovement >= erraticMovementThreshold * 2)
                {
                    TriggerErraticMouseEvent(erraticMouseEventTwo);
                    erraticStage = 2;
                }
                break;

            case 2:
                if (totalErraticMovement >= erraticMovementThreshold * 3)
                {
                    TriggerErraticMouseEvent(erraticMouseEventRespawn);
                    erraticStage = 3;
                    hasCausedErraticRespawn = true;
                }
                break;
        }
    }

    private void TriggerErraticMouseEvent(string eventID)
    {
        if (narratorManager == null) return;

        narratorManager.TriggerEvent(eventID);
    }

    private void ResetErraticSequence()
    {
        totalErraticMovement = 0f;
        erraticMovementAmount = 0f;
        erraticStage = 0;
        directionChanges = 0;
        erraticTimer = 0f;
        timeSinceErraticMovement = 0f;
        hasPreviousMouseDirection = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

        if (lookInput.sqrMagnitude > 0.01f)
        {
            timeSinceErraticMovement = 0f;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
