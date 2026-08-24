using UnityEngine;

public class NarratorTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private NarratorManager narratorManager;
    [SerializeField] private string enterTriggerID;
    [SerializeField] private string exitTriggerID;

    [Header("Exit timing")]
    [SerializeField] private float quickExitTime = 3f;

    private float timeEntered;
    private bool playerInside;

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

        playerInside = true;
        timeEntered = Time.time;

        narratorManager.TriggerEvent(enterTriggerID);
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

        playerInside = false;
        float timeSpendInside = Time.time - timeEntered;

        if (timeSpendInside <= quickExitTime)
        {
            narratorManager.TriggerEvent(exitTriggerID);
        }
    }
}
