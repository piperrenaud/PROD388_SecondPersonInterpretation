using UnityEngine;
using System.Collections;

public class ProtagDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private IntroEnding introEnding;

    [Header("Puzzle related dialogue")]
    [SerializeField] private string[] puzzleOneDialogues;
    [SerializeField] private string[] puzzleTwoDialogues;
    [SerializeField] private string[] puzzleTwoDialogueTwo;
    [SerializeField] private string[] puzzleThreeDialogues;
    [SerializeField] private string[] puzzleThreeDialoguesTwo;

    [Header("Door opened Dialogues")]
    [SerializeField] private string doorOne;
    [SerializeField] private string doorTwo;
    [SerializeField] private string doorThree;

    [Header("Puzzle 2 Question Buttons")]
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private string[] wrongAnswerLines;
    [SerializeField] private OpenDoor playerDoor;
    [SerializeField] private OpenDoor protagDoor;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fiveLeversAudio;

    [SerializeField] private float minimumDelay = 10f;
    [SerializeField] private float maximumDelay = 30f;

    private bool puzzleDialoguePlaying = false;
    private int puzzleCurrentIndex = 0;
    private int currentDoorIndex = 0;

    private Coroutine currentPuzzleRoutine;


    private void Awake()
    {
        buttonParent.SetActive(false);
    }

    public void PlayNestPuzzleLine()
    {
        if (currentPuzzleRoutine != null)
        {
            StopCoroutine(currentPuzzleRoutine);
            currentPuzzleRoutine = null;
        }

        switch (puzzleCurrentIndex)
        {
            case 0:
                currentPuzzleRoutine = StartCoroutine(PlayPuzzleOne());
                puzzleCurrentIndex++;
                break;

            case 1:
                currentPuzzleRoutine = StartCoroutine(PlayPuzzleTwo());
                puzzleCurrentIndex++;
                break;

            case 2:
                currentPuzzleRoutine = StartCoroutine(PlayPuzzleTwoTwo());
                puzzleCurrentIndex++;
                break;

            case 3:
                currentPuzzleRoutine = StartCoroutine(PlayPuzzleThree());
                puzzleCurrentIndex++;
                break;

            case 4:
                currentPuzzleRoutine = StartCoroutine(PlayPuzzleThreeTwo());
                puzzleCurrentIndex++;
                break;
        }
    }

    public void PlayDoorLine()
    {
        switch (currentDoorIndex)
        {
            case 0:
                dialogueManager.SetProtagText(doorOne);
                currentDoorIndex++;
                break;

            case 1:
                dialogueManager.SetProtagText(doorTwo);
                currentDoorIndex++;
                break;

            case 2:
                dialogueManager.SetProtagText(doorThree);
                introEnding.EndLevel();
                currentDoorIndex++;
                break;
        }
    }

    private IEnumerator PlayPuzzleOne()
    {
        if (puzzleOneDialogues == null || puzzleOneDialogues.Length == 0) yield return null;

        puzzleDialoguePlaying = true;

        foreach (string line in puzzleOneDialogues)
        {
            dialogueManager.SetProtagText(line);

            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        puzzleDialoguePlaying = false;
        currentPuzzleRoutine = null;
    }

    private IEnumerator PlayPuzzleTwo()
    {
        if (puzzleTwoDialogues == null || puzzleTwoDialogues.Length == 0) yield return null;

        puzzleDialoguePlaying = true;

        foreach (string line in puzzleTwoDialogues)
        {
            dialogueManager.SetProtagText(line);

            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        puzzleDialoguePlaying = false;
        currentPuzzleRoutine = null;
    }

    private IEnumerator PlayPuzzleTwoTwo()
    {
        if (puzzleTwoDialogueTwo == null || puzzleTwoDialogueTwo.Length == 0) yield return null;

        puzzleDialoguePlaying = true;

        foreach (string line in puzzleTwoDialogueTwo)
        {
            dialogueManager.SetProtagText(line);

            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        puzzleDialoguePlaying = false;

        StartQuestions();
        currentPuzzleRoutine = null;
    }

    private IEnumerator PlayPuzzleThree()
    {
        if (puzzleThreeDialogues == null || puzzleThreeDialogues.Length == 0) yield return null;

        puzzleDialoguePlaying = true;

        foreach (string line in puzzleThreeDialogues)
        {
            dialogueManager.SetProtagText(line);

            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        puzzleDialoguePlaying = false;
        currentPuzzleRoutine = null;
    }

    private IEnumerator PlayPuzzleThreeTwo()
    {
        if (puzzleThreeDialoguesTwo == null || puzzleThreeDialoguesTwo.Length == 0) yield return null;

        puzzleDialoguePlaying = true;

        foreach (string line in puzzleThreeDialoguesTwo)
        {
            dialogueManager.SetProtagText(line);

            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        puzzleDialoguePlaying = false;
        currentPuzzleRoutine = null;
    }

    private void StartQuestions()
    {
        buttonParent.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void WrongAnswer()
    {
        buttonParent.SetActive(false);

        StartCoroutine(WrongAnswerRoutine());
    }

    public void RightAnswer()
    {
        buttonParent.SetActive(false);

        StartCoroutine(RightAnswerRoutine());
    }

    private IEnumerator RightAnswerRoutine()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yield return new WaitForSeconds(2f);

        audioSource.PlayOneShot(fiveLeversAudio);
        yield return new WaitForSeconds(3f);

        playerDoor.UnlockPlayerDoor();
        protagDoor.OpenProtagDoor();

        PlayDoorLine();
    }

    private IEnumerator WrongAnswerRoutine()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yield return new WaitForSeconds(2f);

        audioSource.PlayOneShot(fiveLeversAudio);
        yield return new WaitForSeconds(3f);

        if (dialogueManager.IsDialogueRunning)
        {
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        int randomIndex = Random.Range(0, wrongAnswerLines.Length);

        string currentLine = wrongAnswerLines[randomIndex];

        dialogueManager.SetProtagText(currentLine);

        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        buttonParent.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
