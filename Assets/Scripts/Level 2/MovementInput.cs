using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CommandDialogue
{
    public string word;
    [TextArea(2, 4)] public string dialogue;
}

public class MovementInput : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private TMP_InputField movementInput;
    [SerializeField] private string invalidCommandText = "I don't know what that says";

    [Header("Command List Dictionary")]
    [SerializeField] private TMP_Text commandListText;

    [Header("Reference")]
    [SerializeField] private ProtagonistGridMovement protagGridMovement;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Command Dialogue")]
    [SerializeField] private CommandDialogue[] commandDialogues;

    [Header("Automatic Movement")]
    [SerializeField] private float secondsUntilAutomaticMovment = 10f;

    private float commandTimer;
    private bool automaticMovementRunning = false;

    private HashSet<string> usedCommands = new HashSet<string>();

    private void Start()
    {
        ResetCommandTimer();

        movementInput.ActivateInputField();

        if (commandListText != null)
        {
            commandListText.text = "";
        }
    }

    private void Update()
    {
        //dont count time while dialogue is playing
        if (dialogueManager != null && dialogueManager.IsDialogueRunning)
        {
            return;
        }

        commandTimer += Time.deltaTime;

        if (commandTimer >= secondsUntilAutomaticMovment && !automaticMovementRunning)
        {
            StartCoroutine(AutomaticMove());
        }
    }

    public void SubmitMovementCommand()
    {
        //check if player clicked enter
        if (Keyboard.current == null || !Keyboard.current.enterKey.wasPressedThisFrame)
        {
            return;
        }

        string command = movementInput.text.Trim().ToLower();

        //dont do anything if input empty
        if (string.IsNullOrEmpty(command))
        {
            movementInput.ActivateInputField();
            return;
        }

        //check for invalid commands first
        foreach (CommandDialogue commandDialogue in commandDialogues)
        {
            if (commandDialogue.word.Trim().ToLower() == command)
            {
                AddCommandToList(command);

                dialogueManager.SetProtagText(commandDialogue.dialogue);

                movementInput.text = "";
                movementInput.ActivateInputField();
                return;
            }
        }

        //normal movement commands
        switch (command)
        {
            case "forward":
            case "front":
                AddCommandToList(command);
                protagGridMovement.MoveForward();
                break;

            case "backward":
            case "back":
                AddCommandToList(command);
                protagGridMovement.MoveBackwards();
                break;

            case "left":
                AddCommandToList(command);
                protagGridMovement.MoveLeft();
                break;

            case "right":
                AddCommandToList(command);
                protagGridMovement.MoveRight();
                break;

            default:
                dialogueManager.SetProtagText(invalidCommandText); 
                break;
        }

        //clear input field after submit
        movementInput.text = "";
        movementInput.ActivateInputField();

        ResetCommandTimer();
    }

    private void AddCommandToList(string command)
    {
        command = command.Trim().ToLower();

        if (usedCommands.Contains(command))
        {
            return;
        }

        usedCommands.Add(command);
        string displayCommand = char.ToUpper(command[0]) + command.Substring(1);

        if (string.IsNullOrEmpty(commandListText.text))
        {
            commandListText.text = displayCommand;
        }
        else
        {
            commandListText.text += "\n" + displayCommand;
        }
    }

    private IEnumerator AutomaticMove()
    {
        automaticMovementRunning = true;

        string directionName;

        Vector3? direction = protagGridMovement.GetRandomValidDirection(out directionName);

        //no valid movements avail
        if (!direction.HasValue)
        {
            ResetCommandTimer();
            yield break;
        }

        dialogueManager.SetProtagText("I'm going to try going " + directionName);

        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        //move
        switch (directionName)
        {
            case "forward":
                protagGridMovement.MoveForward();
                break;

            case "backward":
                protagGridMovement.MoveBackwards();
                break;

            case "left":
                protagGridMovement.MoveLeft();
                break;

            case "right":
                protagGridMovement.MoveRight();
                break;
        }

        movementInput.text = "";
        movementInput.ActivateInputField();
    
        ResetCommandTimer();
        automaticMovementRunning = false;
    }

    private void ResetCommandTimer()
    {
        commandTimer = 0f;
    }
}
