using System.Collections;
using UnityEngine;

public class ObjectiveArea : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private CharacterDoorController character;
    [SerializeField] private Inventory inventory;

    [Header("Location")]
    [SerializeField] private Room room;
    [SerializeField] private Transform targetPoint;

    [Header("Objective Dialogue")]
    [SerializeField] private bool objectiveActive = false;
    [SerializeField] private string objectiveDialogue = 
        "Hmm it's locked. I wonder if theres a key back in those cabinets i saw earlier.";

    [Header("Outcome")]
    [SerializeField] private string itemToGive;

    [Header("Other")]
    [SerializeField] private Outline objectiveOutline;
    [SerializeField] private AudioSource audio;

    public bool ObjectiveActive => objectiveActive;
    public Room Room => room;
    public Transform TargetPoint => targetPoint;

    private void Awake()
    {
        SetOutline(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectiveActive)
        {
            StartCoroutine(StartObjective());
        }
    }

    private IEnumerator StartObjective()
    {
        yield return new WaitForSeconds(1f);

        dialogueManager.SetText(objectiveDialogue);
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        //if objective results in item (e.g. key)
        if (!string.IsNullOrEmpty(itemToGive))
        {
            inventory.ObtainedItem(itemToGive, true);
            Debug.Log("Character obtained: " + itemToGive);

            audio.Play();
        }

        ObjectiveManager.Instance.CompleteCurrentObjective();
    }

    public void SetObjectiveActive(bool active)
    {
        objectiveActive = active;
    }

    public void SetOutline(bool outlined)
    {
        objectiveOutline.enabled = outlined;
    }
}
