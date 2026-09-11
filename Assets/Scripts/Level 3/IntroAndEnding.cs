using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAndEnding : MonoBehaviour
{
    [Header("Fade to/from Black")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject fadeParent;

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private CharacterDoorController character;
    [SerializeField] private ObjectiveManager objectiveManager;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private RandomDialogue randomDialogue;

    [Header("Intro Dialogue")]
    [SerializeField] private string[] introLines;

    [Header("End Dialogue")]
    [SerializeField] private string[] endLines;

    private void Awake()
    {
        character.enabled = false;
        objectiveManager.enabled = false;
        cameraSwitcher.enabled = false;
        randomDialogue.enabled = false;

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
            dialogueManager.SetText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        yield return new WaitForSeconds(2f);

        StartLevel();
    }

    private void StartLevel()
    {
        character.enabled = true;
        objectiveManager.enabled = true;
        cameraSwitcher.enabled = true;
        randomDialogue.enabled = true;

        fadeParent.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        EndLevel();
    }

    private IEnumerator EndDialogue()
    {
        yield return new WaitForSeconds(2f);

        animator.SetTrigger("End");
        yield return new WaitForSeconds(4f);

        foreach (string line in endLines)
        {
            dialogueManager.SetText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Level 4");
    }

    private void EndLevel()
    {
        character.enabled = false;
        objectiveManager.enabled = false;
        cameraSwitcher.enabled = false;
        randomDialogue.enabled= false;

        fadeParent.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(EndDialogue());
    }
}
