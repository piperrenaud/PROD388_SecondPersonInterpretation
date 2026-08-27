using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardEventTracker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NarratorManager narratorManager;
    [SerializeField] DialogueManager dialogueManager;

    [Header("ESC Settings")]
    [SerializeField] private float clickRestTime = 15f;
    [SerializeField] private int clicksUntilRespawn = 5;
    [SerializeField] private string escEventOne = "EscEventOne";
    [SerializeField] private string escEventTwo = "EscEventTwo";
    [SerializeField] private string escEventRespawn = "EscEventRespawn";
    [SerializeField] private string postRespawnEvent = "SecondRespawn";

    [Header("Jump Settings")]
    [SerializeField] private string jumpEventOne = "JumpEventOne";
    [SerializeField] private string jumpEventTwo = "JumpEventTwo";
    [SerializeField] private string jumpEventRespawn = "JumpEventRespawn";

    private int escClicks = 0;
    private float escTimer = 0f;
    private bool escHasCausedRespawn = false;

    private int jumpClicks = 0;
    private float jumpTimer = 0f;
    private bool jumpHasCausedRespawn = false;

    private void Update()
    {
        if (Keyboard.current == null) return;

        //esc
        if (escTimer > 0f)
        {
            escTimer -= Time.deltaTime;

            if (escTimer <= 0f)
            {
                escTimer = 0f;
                escClicks = 0;
            }
        }     

        //check for esc pressed
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!dialogueManager.IsDialogueRunning)
            {
                HandleEscClick();
            }
        }

        //jump
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;

            if (jumpTimer <= 0f)
            {
                jumpTimer = 0f;
                jumpClicks = 0;
            }
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!dialogueManager.IsDialogueRunning)
            {
                HandleJumpClick();
            }
        }
    }

    private void HandleEscClick()
    {
        escTimer = clickRestTime;
        escClicks++;

        Debug.Log("Esc clicked: " + escClicks);

        if (escHasCausedRespawn)
        {
            if (escClicks == 1) narratorManager.TriggerEvent(escEventTwo);
            else narratorManager.TriggerEvent(postRespawnEvent);
            return;
        }
        
        if (escClicks == 1) narratorManager.TriggerEvent(escEventOne);
        else if (escClicks < clicksUntilRespawn) narratorManager.TriggerEvent(escEventTwo);
        else if (escClicks == clicksUntilRespawn)
        {
            narratorManager.TriggerEvent(escEventRespawn);
            escClicks = 0;
            escTimer = 0f;
            escHasCausedRespawn = true;
        }
    }

    private void HandleJumpClick()
    {
        jumpTimer = clickRestTime;
        jumpClicks++;

        Debug.Log("Esc clicked: " + jumpClicks);

        if (jumpHasCausedRespawn)
        {
            if (jumpClicks == 1) narratorManager.TriggerEvent(jumpEventTwo);
            else narratorManager.TriggerEvent(postRespawnEvent);
            return;
        }

        if (jumpClicks == 1) narratorManager.TriggerEvent(jumpEventOne);
        else if (jumpClicks < clicksUntilRespawn) narratorManager.TriggerEvent(jumpEventTwo);
        else if (jumpClicks == clicksUntilRespawn)
        {
            narratorManager.TriggerEvent(jumpEventRespawn);
            jumpClicks = 0;
            jumpTimer = 0f;
            jumpHasCausedRespawn = true;
        }
    }
}
