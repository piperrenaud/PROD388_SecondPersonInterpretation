using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    [Header("Narrator")]
    [SerializeField] private NarratorManager narratorManager;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Events")]
    [SerializeField] private string initalEventID;
    [SerializeField] private string repeatedEventID = "WorkerRepeated";
    [SerializeField] private string suspiciousEventID = "WorkerSuspicious";
    [SerializeField] private string respawnEventID = "RespawnPlayer";
    [SerializeField] private string postRespawnEventID = "SecondRespawn";

    [Header("Suspicious Behaviour")]
    [SerializeField] private int interactionsBeforeSuspicious = 3;
    [SerializeField] private float maxTimeBetweenInteractions = 5f;

    [Header("Respawn Behaviour")]
    [SerializeField] private int interactionsBeforeRespawn = 3;

    [Header("Staring Behaviour")]
    [SerializeField] private string stareEventOne = "NPCStareOne";
    [SerializeField] private string stareEventTwo = "NPCStareTwo";
    [SerializeField] private string stareEventThree = "NPCStareThree";
    [SerializeField] private float stareTimeBetweenEvents = 8f;

    private bool hasBeenInteractedWith = false;
    private bool isSuspicious = false;
    private bool hasCausedRespawn = false;

    private int rapidInteractionCount = 0;
    private int suspiciousEventsCount = 0;
    private float interactionTimer = 0f;
    private bool trackingInteractions = false;

    private float stareTimer = 0f;
    private bool isBeingHovered = false;
    private int stareStage = 0;
    private bool waitForDialogue = false;
    private bool stareHasCausedRespawn = false;
    

    private void Update()
    {
        //interation spam timer
        if (trackingInteractions)
        {
            interactionTimer += Time.deltaTime;

            if (interactionTimer > maxTimeBetweenInteractions)
            {
                ResetInteractionSequence();
            }
        }

        //staring timer
        if (isBeingHovered && !waitForDialogue)
        {
            stareTimer += Time.deltaTime;

            if (stareTimer >= stareTimeBetweenEvents)
            {
                StartCoroutine(HandleStareEvent());
            }
        }
    }

    public void Observe()
    {
        if (dialogueManager.IsDialogueRunning) return;

        if (narratorManager == null)
        {
            Debug.LogWarning("NPCInteraction: NarratorManager not assigned: " + gameObject.name);
            return;
        }

        //player already been reset by this object
        if (hasCausedRespawn)
        {
            narratorManager.TriggerEvent(postRespawnEventID);
            return;
        }

        //first ever interaction
        if (!hasBeenInteractedWith)
        {
            hasBeenInteractedWith = true;

            narratorManager.TriggerEvent(initalEventID);
            return;
        }

        //every interaction after the first
        //start/continue spam interaction
        rapidInteractionCount++;
        interactionTimer = 0f;
        trackingInteractions = true;

        //already suspicious
        if (isSuspicious)
        {
            if (rapidInteractionCount >= interactionsBeforeRespawn)
            {
                narratorManager.TriggerEvent(respawnEventID);
                hasCausedRespawn = true;
                ResetInteractionSequence();
                isSuspicious= false;
            }

            return;
        }

        //become suspicious
        if (rapidInteractionCount >= interactionsBeforeSuspicious)
        {
            suspiciousEventsCount++;
            narratorManager.TriggerEvent(suspiciousEventID);

            if (suspiciousEventsCount >= 2)
            {
                isSuspicious = true;
                rapidInteractionCount = 0;
            }

            return;
        }

        //normal repeat
        narratorManager.TriggerEvent(repeatedEventID);
    }

    private void ResetInteractionSequence()
    {
        rapidInteractionCount = 0;
        interactionTimer = 0f;
        trackingInteractions = false;
    }

    public void SetHovered(bool hovered)
    {
        isBeingHovered = hovered;

        if (!hovered)
        {
            stareTimer = 0f;
            waitForDialogue = false;
        }
    }

    private IEnumerator HandleStareEvent()
    {
        waitForDialogue = true;
        stareTimer = 0f;

        //player already reset from staring?
        if (stareHasCausedRespawn)
        {
            narratorManager.TriggerEvent(postRespawnEventID);

            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

            waitForDialogue = false;
            yield break;
        }

        //normal staring progression
        switch (stareStage)
        {
            case 0:
                narratorManager.TriggerEvent(stareEventOne);
                stareStage++;
                break;

            case 1:
                narratorManager.TriggerEvent(stareEventTwo);
                stareStage++;
                break;

            case 2:
                narratorManager.TriggerEvent(stareEventThree);
                stareStage++;
                break;

            case 3:
                narratorManager.TriggerEvent(respawnEventID);
                stareStage++;
                stareHasCausedRespawn = true;
                break;
        }

        //wait for dialogue to finish
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        stareTimer = 0f;
        waitForDialogue = false;
    }
}
