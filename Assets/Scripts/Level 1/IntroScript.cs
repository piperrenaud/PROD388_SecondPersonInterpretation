using System.Collections;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    [Header("To Disable")]
    [SerializeField] private FirstPersonMovement playerMovement;

    [Header("References")]
    [SerializeField] private NarratorManager narratorManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Animator fadeToBlackAnimator;

    [Header("Events")]
    [SerializeField] private string introEventOne = "IntroOne";
    [SerializeField] private string introEventTwo = "IntroTwo";
    [SerializeField] private string introEventThree = "IntroThree";
    [SerializeField] private string introEventFour = "IntroFour";
    [SerializeField] private string introEventFive = "IntroFive";
    [SerializeField] private string introEventSix = "IntroSix";


    private void Start()
    {
        dialogueManager.SetTimeAfterText(4f);

        ToggleMovement(false);
        StartCoroutine(Intro());   
    }

    private IEnumerator Intro()
    {
        fadeToBlackAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(4.5f);

        narratorManager.TriggerEvent(introEventOne);
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        narratorManager.TriggerEvent(introEventTwo);
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        narratorManager.TriggerEvent(introEventThree);
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        narratorManager.TriggerEvent(introEventFour);
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        narratorManager.TriggerEvent(introEventFive);
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        narratorManager.TriggerEvent(introEventSix);
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        ToggleMovement(true);

        dialogueManager.SetTimeAfterText(2.5f);
    }

    private void ToggleMovement(bool value)
    {
        playerMovement.enabled = value;
    }
}
