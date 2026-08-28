using UnityEngine;

public class NarratorTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private NarratorManager narratorManager;
    [SerializeField] private string enterTriggerID;
    [SerializeField] private string exitTriggerID;

    [Header("Danger Sign?")]
    [SerializeField] private DangerInteract[] dangerSigns;
    [SerializeField] private string seenSignEnterTriggerID;

    [Header("Exit timing")]
    [SerializeField] private float quickExitTime = 3f;

    private float timeEntered;
    private bool playerAlreadyExplored;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (enterTriggerID == "Office")
        {
            narratorManager.CheckOfficesEntered();
            return;
        }

        timeEntered = Time.time;

        //check if playes seen danger sign
        if (HasSeenDangerSign())
        {
            narratorManager.TriggerEvent(seenSignEnterTriggerID);
        }
        else
        {
            narratorManager.TriggerEvent(enterTriggerID);
        }
    }

    private bool HasSeenDangerSign()
    {
        foreach (DangerInteract sign in dangerSigns)
        {
            if (sign != null && sign.HasPlayerSeen)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (exitTriggerID == null)
        {
            return;
        }

        if (playerAlreadyExplored) return;

        playerAlreadyExplored = true;
        float timeSpendInside = Time.time - timeEntered;

        if (timeSpendInside <= quickExitTime)
        {
            narratorManager.TriggerEvent(exitTriggerID);
        }
    }
}
