using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class LevelOneInteractions : MonoBehaviour
{
    [Header("Door Sounds")]
    [SerializeField] private AudioClip[] openDoorSounds;
    [SerializeField] private AudioClip[] closeDoorSounds;

    [Header("Sound Effect Settings")]
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Narrator")]
    [SerializeField] private NarratorManager narratorManager;

    [Header("Experiment")]
    [SerializeField] private float timeUntilExperiment = 30f;
    [SerializeField] private float timeUntilReminder = 20f;
    [SerializeField] private float timeUntilForce = 20f;

    [SerializeField] private string experimentStartingSoon = "ExperimentStartingSoon";
    [SerializeField] private string experimentReminder = "ExperimentReminder";
    [SerializeField] private string experimentForce = "ExperimentForce";

    [Header("Facility")]
    [SerializeField] private Transform facilityRoomSpawn;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private NPCManager npcManager;
    [SerializeField] private NarratorTrigger facilityRoomTrigger;

    [SerializeField] private Animator experimentAnimation;

    private bool dangerSignInteracted = false;
    private DangerInteract lastInteractedSign;

    private Coroutine experimentCoroutine;
    private bool playerEnteredFacility;
    private bool experimentStarted;

    private bool timerUp = false;
    public bool IsTimerUp => timerUp;

    private void Start()
    {
        experimentCoroutine = StartCoroutine(ExperimentSequence());
    }

    public void HandleInteraction(GameObject interactedObject)
    {
        if (interactedObject.CompareTag("Door"))
        {
            HandleDoor(interactedObject);
        }

        if (interactedObject.CompareTag("LightSwitch"))
        {
            HandleLights(interactedObject);
        }

        if (interactedObject.CompareTag("Object"))
        {
            HandleObject(interactedObject);
        }

        if (interactedObject.CompareTag("NPC"))
        {
            HandleNPC(interactedObject);
        }

        if (interactedObject.CompareTag("DangerSign"))
        {
            HandleDangerSign(interactedObject);
        }
    }

    private void HandleDoor(GameObject door)
    {
        Animator anim = door.GetComponentInParent<Animator>();
        AudioSource source = door.GetComponentInParent<AudioSource>();

        if (anim == null)
        {
            Debug.Log("door has no animator");
            return;
        }

        bool isOpen = anim.GetBool("IsOpen");

        DoorNarratorTracker tracker = door.GetComponentInParent<DoorNarratorTracker>();

        if (isOpen)
        {
            PlaySound(closeDoorSounds, source);

            anim.SetBool("IsOpen", false);

            if (tracker != null) tracker.DoorClosed();
        }
        else
        {
            PlaySound(openDoorSounds, source);

            anim.SetBool("IsOpen", true);

            if (tracker != null) tracker.DoorOpened();
        }
    }

    private void PlaySound(AudioClip[] clips, AudioSource source)
    {
        if (clips == null || clips.Length == 0 || source == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip == null) return;

        source.pitch = Random.Range(minPitch, maxPitch);
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 1.5f;
        source.maxDistance = 12f;
        source.dopplerLevel = 0f;

        source.PlayOneShot(clip, volume);
    }

    private void HandleLights(GameObject interactedObject)
    {
        LightSwitch lightSwitch = interactedObject.GetComponent<LightSwitch>();

        lightSwitch.ToggleLights();
    }

    private void HandleObject(GameObject interactedObject)
    {
        ObjectInteraction objectInteraction = interactedObject.GetComponent<ObjectInteraction>();

        if (objectInteraction != null)
        {
            objectInteraction.Observe();
        }
    }

    private void HandleNPC(GameObject interactedObject)
    {
        NPCInteraction npc = interactedObject.GetComponent<NPCInteraction>();

        if (npc == null)
        {
            Debug.LogWarning("NPC has no NPCInteraction component");
            return;
        }

        npc.Observe();
    }

    private void HandleDangerSign(GameObject interactedObject)
    {
        DangerInteract dangerInteract = interactedObject.GetComponent<DangerInteract>();

        if (dangerInteract == null) return;

        dangerInteract.Observe();
    }

    private IEnumerator ExperimentSequence()
    {
        //wait for experiment to start
        yield return new WaitForSeconds(timeUntilExperiment);
        timerUp = true;

        npcManager.GatherNPCs();
        narratorManager.TriggerEvent(experimentStartingSoon);

        if (facilityRoomTrigger.IsPlayerInside())
        {
            yield return new WaitForSeconds(3f);
            PlayerEnteredFacility();
            yield break;
        }

        if (playerEnteredFacility)
        {
            yield break;
        }


        //wait for player to get to facility room
        yield return new WaitForSeconds(timeUntilReminder);

        if (playerEnteredFacility) yield break;

        narratorManager.TriggerEvent(experimentReminder);

        //wait again
        yield return new WaitForSeconds(timeUntilForce);

        if (playerEnteredFacility) yield break; 

        narratorManager.TriggerEvent(experimentForce);

        //force player to facility room
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        if (playerEnteredFacility) yield break;

        TeleportPlayerToFacility();
        experimentCoroutine = null;
    }

    public void PlayerEnteredFacility()
    {
        if (playerEnteredFacility || experimentStarted) return;

        playerEnteredFacility = true;
        experimentStarted = true;

        //stop experiment sequence
        if (experimentCoroutine != null)
        {
            StopCoroutine(experimentCoroutine);
            experimentCoroutine = null;
        }

        StartCoroutine(FailedExperiment());
    }

    private void TeleportPlayerToFacility()
    {
        if (playerState == null) return;
        if (facilityRoomSpawn == null) return;

        GameObject player = playerState.gameObject;

        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.position = facilityRoomSpawn.position;
        player.transform.rotation = facilityRoomSpawn.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }

        StartFailedExperiment();
    }

    private IEnumerator FailedExperiment()
    {
        yield return new WaitForSeconds(3f);

        Debug.Log("starting cutscene");

        playerState.gameObject.SetActive(false);
        experimentAnimation.SetTrigger("Start");
    }

    private void StartFailedExperiment()
    {
        if (experimentStarted) return;

        experimentStarted = true;
        StartCoroutine(FailedExperiment());
    }
}
