using UnityEngine;

public class DoorNarratorTracker : MonoBehaviour
{
    [Header("Narrator")]
    [SerializeField] private NarratorManager narratorManager;

    [Header("Settings")]
    [SerializeField] private int interactionsBeforeNarration = 6;
    [SerializeField] private float maxTimeBetweenInteractions = 2f;

    private int interactionCount = 0;
    private float interactionTimer = 0f;

    private bool lastInteractionWasOpen = false;
    private bool hasStartedSequence = false;

    private void Update()
    {
        if (!hasStartedSequence)
        {
            return;
        }

        interactionTimer += Time.deltaTime;

        if (interactionTimer > maxTimeBetweenInteractions)
        {
            ResetSequence();
        }
    }

    public void DoorOpened()
    {
        if (hasStartedSequence && lastInteractionWasOpen)
        {
            ResetSequence();
        }

        interactionCount++;

        lastInteractionWasOpen= true;
        hasStartedSequence = true;
        interactionTimer = 0f;

        CheckForRepeatedInteraction();
    }

    public void DoorClosed()
    {
        if (hasStartedSequence && !lastInteractionWasOpen)
        {
            ResetSequence();
        }

        interactionCount++;

        lastInteractionWasOpen = false;
        hasStartedSequence = true;
        interactionTimer = 0f;

        CheckForRepeatedInteraction();
    }

    private void CheckForRepeatedInteraction()
    {
        if (interactionCount >= interactionsBeforeNarration)
        {
            narratorManager.DoorSequenceCompleted();
            ResetSequence();
        }
    }

    private void ResetSequence()
    {
        interactionCount = 0;
        interactionTimer = 0f;
        hasStartedSequence= false;
    }
}
