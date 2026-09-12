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

    private Dictionary<string, List<string>> usedDialogueLines = new Dictionary<string, List<string>>();

    private int doorSequences = 0;
    private int lightSequences = 0;
    private int officesEntered = 0;

    //current event
    private NarratorEventData currentEvent;
    private int currentPriority = -1;

    //stop olf respawn coroutines form acting
    private Coroutine respawnCoroutine;

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

        //priorty check
        if (dialogueManager.IsDialogueRunning)
        {
            //new event has lower priority
            if (eventData.priority < currentPriority)
            {
                Debug.Log($"ignoring {eventID} because its lower priority than {currentEvent.eventID}");
                return;
            }    

            //same or higher priorty takes over
            Debug.Log($"interrupting {currentEvent.eventID} with {eventID}");
        }

        string dialogue = GetUnusedDialogue(eventData);

        if (string.IsNullOrEmpty(dialogue))
        {
            Debug.Log("No unused dialogue left for narrator event: " + eventID);
            return;
        }

        if (!eventData.canRepeat) completedEvents.Add(eventID);

        //cancel any old respawn waiting for the previous dialogue
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }

        //set this as new active event
        currentEvent = eventData;
        currentPriority = eventData.priority;

        //interrupt current dialogue
        dialogueManager.SetProtagText(dialogue);

        //respawn events wait for this dialogue to finish
        if (eventID.Contains("Respawn"))
        {
            respawnCoroutine = StartCoroutine(RespawnPlayer());
        }
    }

    private string GetUnusedDialogue(NarratorEventData eventData)
    {
        if (eventData.dialogueLines == null || eventData.dialogueLines.Count == 0)
        {
            Debug.LogWarning("Narrator event has no dialogue lines: " + eventData.eventID);
            return null;
        }

        //used lines for this event
        if (!usedDialogueLines.ContainsKey(eventData.eventID))
        {
            usedDialogueLines[eventData.eventID] = new List<string>();
        }

        List<string> usedLines = usedDialogueLines[eventData.eventID];

        //lines that havent been used
        List<string> availableLines = new List<string>();

        foreach (string line in eventData.dialogueLines)
        {
            if (!usedLines.Contains(line))
            {
                availableLines.Add(line);
            }
        }

        //ever line has already been used
        if (availableLines.Count == 0) return null;

        string selectedLine = availableLines[Random.Range(0, availableLines.Count)];

        //remeber that this lines been used
        usedLines.Add(selectedLine);

        return selectedLine;
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        //clear active event
        currentEvent = null;
        currentPriority = -1;

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
