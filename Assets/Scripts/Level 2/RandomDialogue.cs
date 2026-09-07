using System.Collections;
using UnityEngine;

public class RandomDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Random Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string[] dialogueLines;

    [SerializeField] private float minimumDelay = 10f;
    [SerializeField] private float maximumDelay = 30f;

    private bool isRunning = false;
    private int lastDialogueIndex = -1;

    private void Start()
    {
        StartCoroutine(RandomDialogueLoop());
    }

    private IEnumerator RandomDialogueLoop()
    {
        isRunning = true;

        while (isRunning)
        {
            //wait random amt of time
            float delay = Random.Range(minimumDelay, maximumDelay);
            yield return new WaitForSeconds(delay);

            if (dialogueLines.Length == null || dialogueLines.Length == 0) continue;

            if (dialogueManager.IsDialogueRunning)
            {
                yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
            }

            //pick random line
            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, dialogueLines.Length);
            }
            while (dialogueLines.Length > 1 && randomIndex == lastDialogueIndex);

            lastDialogueIndex = randomIndex;

            string randomLine = dialogueLines[randomIndex];


            dialogueManager.SetText(randomLine);

            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }
    }
}
