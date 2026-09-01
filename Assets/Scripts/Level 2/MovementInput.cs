using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Reference")]
    [SerializeField] private ProtagonistGridMovement protagGridMovement;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Command Dialogue")]
    [SerializeField] private CommandDialogue[] commandDialogues;

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
                dialogueManager.SetText(commandDialogue.dialogue);

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
                protagGridMovement.MoveForward();
                break;

            case "backward":
            case "back":
                protagGridMovement.MoveBackwards();
                break;

            case "left":
                protagGridMovement.MoveLeft();
                break;

            case "right":
                protagGridMovement.MoveRight();
                break;

            default:
                dialogueManager.SetText(invalidCommandText); 
                break;
        }

        //clear input field after submit
        movementInput.text = "";
        movementInput.ActivateInputField();
    }
}
