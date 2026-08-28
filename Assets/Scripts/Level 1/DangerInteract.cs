using UnityEngine;
using System.Collections;

public class DangerInteract : MonoBehaviour
{
    [Header("Narrator")]
    [SerializeField] private NarratorManager narratorManager;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Shared Danger Sign Group")]
    [SerializeField] private DangerSignGroup dangerSignGroup;

    [Header("This Sign")]
    [SerializeField] private string initialEventID;

    private bool hasBeenInteractedWith = false;
    private bool playerHasSeen = false;

    public bool HasPlayerSeen => playerHasSeen;

    public void Observe()
    {
        if (dialogueManager.IsDialogueRunning) return;

        if (narratorManager == null)
        {
            return;
        }

        playerHasSeen = true;
        
        //already been repsawned by sign?
        if (dangerSignGroup.HasCausedRespawn)
        {
            dangerSignGroup.HandlePostRespawn();
            return;
        }

        //first interacion
        if (!hasBeenInteractedWith)
        {
            hasBeenInteractedWith = true;

            dangerSignGroup.FirstInteraction(initialEventID);
            return;
        }

        dangerSignGroup.Interact();
    }

    public void SetHovered(bool hovered)
    {
        if (hovered)
        {
            playerHasSeen = true;
        }
    }
}
