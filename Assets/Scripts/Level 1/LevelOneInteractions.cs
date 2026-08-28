using UnityEngine;

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
}
