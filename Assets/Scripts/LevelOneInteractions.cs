using UnityEngine;

public class LevelOneInteractions : MonoBehaviour
{
    [Header("Door Sounds")]
    public AudioClip[] openDoorSounds;
    public AudioClip[] closeDoorSounds;

    [Header("Sound Effect Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    public void HandleInteraction(GameObject interactedObject)
    {
        if (interactedObject.CompareTag("Door"))
        {
            HandleDoor(interactedObject);
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

}
