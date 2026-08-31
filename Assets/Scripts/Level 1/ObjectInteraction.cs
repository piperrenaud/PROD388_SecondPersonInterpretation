using UnityEngine;
using System.Collections;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Narrator")]
    [SerializeField] private NarratorManager narratorManager;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Interacting Behaviour")]
    [SerializeField] private string initalEventID;
    [SerializeField] private string stageOneEvent = "StageOne";
    [SerializeField] private string stageTwoEvent = "StageTwo";
    [SerializeField] private string interactedEvent;
    [SerializeField] private string postInterectEvent;
    [SerializeField] private string postInteractRepeated;
    [SerializeField] private string respawnEvent = "RespawnPlayer";
    [SerializeField] private string postRepawnEvent = "SecondRespawn";

    [Header("Suspicious Behaviour")]
    [SerializeField] private int interactionsBeforeNextStage = 3;
    [SerializeField] private float maxTimeBetweenInteractions = 5f;

    [Header("Respawn Behaviour")]
    [SerializeField] private int interactionsBeforeRespawn = 3;

    private bool hasBeenInteractedWith = false;
    private bool isAtFinalStage = false;
    private bool hasHadFinalInteraction = false;
    private bool hasBeenRespawned = false;

    private int rapidInteractionCount = 0;
    private int interactionsAtFinalStage = 0;
    private float interactionTimer = 0f;
    private bool trackingInteractions = false;
    private int postRespawnInteractionsCount = 0;

    private bool buttonInteracted = false;
    private bool vaseInteracted = false;
    private bool robotInteracted = false;

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
        if (hasHadFinalInteraction)
        {
            if (postRespawnInteractionsCount == 0)
            {
                narratorManager.TriggerEvent(postInterectEvent);
                postRespawnInteractionsCount++;
                return;
            }
            else if (postRespawnInteractionsCount < 4)
            {
                narratorManager.TriggerEvent(postInteractRepeated);
                postRespawnInteractionsCount++;
                return;
            }
            else if (postRespawnInteractionsCount >= 4)
            {
                narratorManager.TriggerEvent(respawnEvent);
                hasBeenRespawned = true;
                return;
            }

            if (hasBeenRespawned)
            {
                narratorManager.TriggerEvent(postRepawnEvent);
            }
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
        if (isAtFinalStage)
        {
            if (rapidInteractionCount >= interactionsBeforeRespawn)
            {
                HandleObjectInteracted();
                narratorManager.TriggerEvent(interactedEvent);
                hasHadFinalInteraction = true;
                ResetInteractionSequence();
                isAtFinalStage = false;
            }

            return;
        }

        //become suspicious
        if (rapidInteractionCount >= interactionsBeforeNextStage)
        {
            interactionsAtFinalStage++;
            narratorManager.TriggerEvent(stageTwoEvent);

            if (interactionsAtFinalStage >= 2)
            {
                isAtFinalStage = true;
                rapidInteractionCount = 0;
            }

            //if vase
            if (gameObject.name.Contains("Vase"))
            {
                GetComponent<Animator>().SetTrigger("Knock");
            }

            return;
        }

        //normal repeat
        narratorManager.TriggerEvent(stageOneEvent);
    }

    private void ResetInteractionSequence()
    {
        rapidInteractionCount = 0;
        interactionTimer = 0f;
        trackingInteractions = false;
    }

    private void HandleObjectInteracted()
    {
        //red light
        if (gameObject.name.Contains("Red Light"))
        {
            Light light = GetComponentInChildren<Light>();
            light.color = Color.red;

            buttonInteracted = true;
        }

        //vase
        if (gameObject.name.Contains("Vase"))
        {
            Animator anim = GetComponent<Animator>();
            anim.SetTrigger("Fall");

            vaseInteracted = true;
        }

        //mechanical arm
        if (gameObject.name.Contains("Mechanical arm"))
        {
            GetComponent<Animator>().enabled = false;

            ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
            particles.Play();

            AudioSource whirSource = GetComponent<AudioSource>();   
            whirSource.enabled = false;

            AudioSource sparkSource = transform.Find("ElectricitySparks").GetComponent<AudioSource>();
            sparkSource.enabled = true;

            robotInteracted = true;
        }
    }
}
