using System.Collections;
using UnityEngine;

public class LinearDialogueLines : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string[] dialogueLines;

    [SerializeField] private float minimumDelay = 10f;
    [SerializeField] private float maximumDelay = 30f;

    private bool isRunning = false;
    private int currentIndex = 0;

    private void Start()
    {
        StartCoroutine(DialogueLoop());
    }

    private IEnumerator DialogueLoop()
    {
        isRunning = true;

        while (isRunning)
        {
            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                yield return null;
                continue;
            }

            //wait random amt of time
            float delay = Random.Range(minimumDelay, maximumDelay);
            yield return new WaitForSeconds(delay);

            if (dialogueManager.IsDialogueRunning)
            {
                yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
            }

            string currentLine = dialogueLines[currentIndex];

            dialogueManager.SetProtagText(currentLine);

            currentIndex++;

            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }
    }
}
