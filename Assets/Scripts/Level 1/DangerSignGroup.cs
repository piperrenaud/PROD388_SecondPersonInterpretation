using UnityEngine;

public class DangerSignGroup : MonoBehaviour
{
    [Header("Shared Interaction Settings")]
    [SerializeField] private int interactionsBeforeSuspicious = 3;
    [SerializeField] private float maxTimeBetweenInteractions = 5f;
    [SerializeField] private int interactionsBeforeRespawn = 3;

    [Header("Narrator")]
    [SerializeField] private NarratorManager narratorManager;

    [Header("Events")]
    [SerializeField] private string repeatEvent;
    [SerializeField] private string suspiciousEvent;
    [SerializeField] private string respawnEvent = "RespawnPlayer";
    [SerializeField] private string postRespawnEvent = "SecondRespawn";

    private int rapidInteractionCount = 0;
    private int suspiciousEventsCount = 0;

    private float interactionTimer = 0f;

    private bool trackingInteractions = false;
    private bool isSuspicious = false;
    private bool hasCausedRespawn = false;

    private void Update()
    {
        if (!trackingInteractions) return;

        interactionTimer += Time.deltaTime;

        if (interactionTimer > maxTimeBetweenInteractions)
        {
            ResetInteractionSequence();
        }
    }

    public void Interact()
    {
        interactionTimer = 0f;
        trackingInteractions = true;

        //already sus
        if (isSuspicious)
        {
            rapidInteractionCount++;

            if (rapidInteractionCount >= interactionsBeforeRespawn)
            {
                narratorManager.TriggerEvent(respawnEvent);

                hasCausedRespawn = true;
                ResetInteractionSequence();
                isSuspicious = false;
            }

            return;
        }

        rapidInteractionCount++;

        //become sus
        if (rapidInteractionCount >= interactionsBeforeSuspicious)
        {
            suspiciousEventsCount++;

            narratorManager.TriggerEvent(suspiciousEvent);

            if (suspiciousEventsCount >= 2)
            {
                isSuspicious = true;
                rapidInteractionCount = 0;
            }

            return;
        }

        //noraml repeat
        narratorManager.TriggerEvent(repeatEvent);
    }

    public void FirstInteraction(string eventID)
    {
        narratorManager.TriggerEvent(eventID);
    }

    public void HandlePostRespawn()
    {
        if (hasCausedRespawn)
        {
            narratorManager.TriggerEvent(postRespawnEvent);
        }
    }

    private void ResetInteractionSequence()
    {
        rapidInteractionCount = 0;
        interactionTimer = 0f;
        trackingInteractions = false;
    }

    public bool HasCausedRespawn => hasCausedRespawn;
}
