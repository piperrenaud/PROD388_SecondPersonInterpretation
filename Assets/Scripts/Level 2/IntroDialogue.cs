using System.Collections;
using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inputTextBox;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Animator fadeFromBlackAnimator;
    [SerializeField] private Animator protagAnimator;
    [SerializeField] private RandomDialogue randomDialogue;
    [SerializeField] private MovementInput movementInput;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string[] lines;

    [Header("Instructions")]
    [SerializeField] private string[] instructions;

    private void Awake()
    {
        inputTextBox.SetActive(false);
        fadeFromBlackAnimator.gameObject.SetActive(true);
        randomDialogue.enabled = false;
        movementInput.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(StartingScene());
    }

    private IEnumerator StartingScene()
    {
        yield return new WaitForSeconds(2f);

        fadeFromBlackAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(1f);
        protagAnimator.SetTrigger("GetUp");

        yield return new WaitForSeconds(5f);

        foreach (var line in lines)
        {
            dialogueManager.SetProtagText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        StartScene();
    }

    private void StartScene()
    {
        inputTextBox.SetActive(true);
        fadeFromBlackAnimator.gameObject.SetActive(false);
        movementInput.enabled = true;

        StartCoroutine(Instructions());
    }

    private IEnumerator Instructions()
    {
        foreach (var line in instructions)
        {
            dialogueManager.SetPlayerText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        randomDialogue.enabled = true;
    }
}
