using UnityEngine;
using static Unity.VisualScripting.Member;

public class OpenDoor : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Locked?")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool needsKey = false;
    [SerializeField] private bool hasKey = false;

    [Header("Character")]
    [SerializeField] private PlayerMovement character;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Audio")]
    [SerializeField] private AudioClip[] openDoorSounds;
    [SerializeField] private AudioClip[] closeDoorSounds;
    [SerializeField] private AudioClip[] unlockDoorSounds;
    [SerializeField] private AudioClip[] lockedDoorSounds;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    public bool IsLocked => isLocked;
    public bool IsOpen => isOpen;

    private bool isOpen = false;
    private int lastInteractionFrame = -1;

    public void Interact()
    {
        if (lastInteractionFrame == Time.frameCount) return;

        lastInteractionFrame = Time.frameCount;

        if (isLocked)
        {
            AudioSource source = GetComponent<AudioSource>();
            PlaySound(lockedDoorSounds, source);

            return;
            
        }

        ToggleDoor();
    }

    private void ToggleDoor()
    {
        if (needsKey && !hasKey) return;

        if (animator == null)
        {
            Debug.LogError("No animator on door");
            return;
        }

        AudioSource source = GetComponent<AudioSource>();

        if (isOpen)
        {
            PlaySound(closeDoorSounds, source);
            animator.SetTrigger("Close");
            isOpen = false;
        }
        else
        {
            PlaySound(openDoorSounds, source);
            animator.SetTrigger("Open");
            isOpen = true;
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

    public void UnlockPlayerDoor()
    {
        isLocked = false;

        AudioSource source = GetComponent<AudioSource>();
        PlaySound(unlockDoorSounds, source);
    }

    public void OpenProtagDoor()
    {
        isLocked = false;

        AudioSource source = GetComponent<AudioSource>();

        PlaySound(openDoorSounds, source);
        animator.SetTrigger("Open");
        isOpen = true;
    }

    public void KeyPickedUp()
    {
        isLocked = false;
        hasKey = true;

        AudioSource source = GetComponent<AudioSource>();
        PlaySound(unlockDoorSounds, source);
    }

    public void Open()
    {
        isLocked = false;
        needsKey = false;

        AudioSource source = GetComponent<AudioSource>();

        PlaySound(openDoorSounds, source);
        animator.SetTrigger("Open");
        isOpen = true;
    }

    public void LockedEndDoor()
    {
        AudioSource source = GetComponent<AudioSource>();
        PlaySound(lockedDoorSounds, source);

        ProtagDialogue protagDialogue = FindFirstObjectByType<ProtagDialogue>();

        if (protagDialogue != null )
        {
            protagDialogue.PlayNestPuzzleLine();
        }
    }
}
