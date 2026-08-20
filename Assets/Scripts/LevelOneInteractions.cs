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

        if (interactedObject.CompareTag("DialogueProp"))
        {
            HandleDialogue(interactedObject);
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

        if (isOpen)
        {
            PlaySound(closeDoorSounds, source);
        }
        else
        {
            PlaySound(openDoorSounds, source);
        }

        anim.SetBool("IsOpen", !isOpen);
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

    private void HandleDialogue(GameObject interactedObject)
    {
        dialogueManager.SetText($"This is a {interactedObject.name}");
    }
}
