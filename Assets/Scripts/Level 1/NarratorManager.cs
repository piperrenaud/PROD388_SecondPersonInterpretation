using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NarratorManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private Transform playerSpawn;

    [Header("Narrator Events")]
    [SerializeField] private List<NarratorEventData> narratorEvents;

    private HashSet<string> completedEvents = new HashSet<string>();

    private int doorSequences = 0;
    private int lightSequences = 0;
    private int officesEntered = 0;

    public void DoorSequenceCompleted()
    {
        doorSequences++;

        if (doorSequences == 1) TriggerEvent("RepeatedDoorInteractionOne");
        else if (doorSequences == 2) TriggerEvent("RepeatedDoorInteractionTwo");
        else if (doorSequences == 3) TriggerEvent("RepeatedDoorInteractionThree");
        else TriggerEvent("RespawnPlayer");
    }

    public void LightSequenceCompleted()
    {
        lightSequences++;

        if (lightSequences == 1) TriggerEvent("RepeatedLightsInteractionOne");
        else if (lightSequences == 2) TriggerEvent("RepeatedLightsInteractionTwo");
        else if (lightSequences == 3) TriggerEvent("RepeatedLightsInteractionThree");
        else TriggerEvent("RespawnPlayer");
    }

    public void CheckOfficesEntered()
    {
        officesEntered++;

        Debug.Log("Offices entered: " + officesEntered);

        switch (officesEntered)
        {
            case 1:
                TriggerEvent("FirstOfficeEntered");
                break;

            case 2:
                TriggerEvent("SecondOfficeEntered");
                break;

            case 3:
                TriggerEvent("ThirdOfficeEntered");
                break;

            case 4:
                TriggerEvent("FourthOfficeEntered");
                break;
        }
    }

    public void TriggerEvent(string eventID)
    {
        if (dialogueManager == null)
        {
            Debug.LogError("NarratorManager: DialogueManager not assigned");
            return;
        }

        if (narratorEvents == null)
        {
            Debug.LogError("NarratorManager: Narrator Events list is null");
            return;
        }

        NarratorEventData eventData = narratorEvents.Find(
            x => x.eventID == eventID);

        if (eventData == null)
        {
            Debug.LogWarning("Narrator event not found: " + eventID);
            return;
        }

        if (!eventData.canRepeat && completedEvents.Contains(eventID))
        {
            return;
        }

        completedEvents.Add(eventID);

        dialogueManager.SetText(eventData.dialogue);

        if (eventID == "RespawnPlayer")
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(3f);

        if (playerState == null)
        {
            Debug.Log("NarratorManager: PlayerState not assigned");
            yield break;
        }

        GameObject player = playerState.gameObject;
        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.position = playerSpawn.position;
        player.transform.rotation = playerSpawn.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }
    }
}
