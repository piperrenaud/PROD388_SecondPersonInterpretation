using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroEnding : MonoBehaviour
{
    [Header("Fade to/from Black")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject fadeParent;

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] ProtagDialogue protagDialogue;
    

    [Header("Intro Dialogue")]
    [SerializeField] private string[] introLines;

    [Header("End Dialogue")]
    [SerializeField] private string[] endLines;

    [Header("Instructions")]
    [SerializeField] private string[] instructions;

    private void Awake()
    {
        playerMovement.DisableMovement();
        protagDialogue.enabled = false;

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
        playerMovement.EnableMovement();
        protagDialogue.enabled = true;

        fadeParent.SetActive(false);

        StartCoroutine(Instructions());
    }

    private IEnumerator Instructions()
    {
        foreach (string line in instructions)
        {
            dialogueManager.SetPlayerText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }
    }

    private IEnumerator EndDialogue()
    {
        yield return new WaitForSeconds(2f);

        playerMovement.DisableMovement();
        protagDialogue.enabled = false;

        foreach (string line in endLines)
        {
            dialogueManager.SetProtagText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        yield return new WaitForSeconds(2f);

        animator.SetTrigger("End");
        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene("Level 6");
    }

    public void EndLevel()
    {
        fadeParent.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(EndDialogue());
    }
}
