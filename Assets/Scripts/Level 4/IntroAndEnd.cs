using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAndEnd : MonoBehaviour
{
    [Header("Fade to/from Black")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject fadeParent;

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private SpiritController playerMovement;
    [SerializeField] private ObjectPossession objectPosession;
    [SerializeField] private ProtagonistWander protagWander;
    [SerializeField] private LinearDialogueLines linearDialogueLines;

    [Header("Intro Dialogue")]
    [SerializeField] private string[] introLines;

    [Header("End Dialogue")]
    [SerializeField] private string[] endLines;

    [Header("Instructions")]
    [SerializeField] private string[] instructions;

    private void Awake()
    {
        playerMovement.enabled = false;
        objectPosession.enabled = false;
        linearDialogueLines.enabled = false;
        protagWander.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fadeParent.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(StartDialogue());
    }

    private IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(2f);

        animator.SetTrigger("Start");
        yield return new WaitForSeconds(5f);

        foreach (string line in introLines)
        {
            dialogueManager.SetProtagText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        yield return new WaitForSeconds(2f);

        StartLevel();
    }

    private void StartLevel()
    {
        objectPosession.enabled = true;
        playerMovement.enabled = true;
        protagWander.enabled = true;

        fadeParent.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(Instructions());
    }

    private IEnumerator Instructions()
    {
        foreach (string line in instructions)
        {
            dialogueManager.SetPlayerText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        linearDialogueLines.enabled = true;
    }

    private IEnumerator EndDialogue()
    {
        yield return new WaitForSeconds(2f);

        animator.SetTrigger("End");
        yield return new WaitForSeconds(4f);

        foreach (string line in endLines)
        {
            dialogueManager.SetProtagText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Level 5");
    }

    public void EndLevel()
    {
        objectPosession.enabled = false;
        playerMovement.enabled = false;
        linearDialogueLines.enabled = false;
        protagWander.enabled = false;

        fadeParent.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(EndDialogue());
    }
}
