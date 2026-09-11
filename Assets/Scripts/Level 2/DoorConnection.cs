using UnityEngine;

public class DoorConnection : MonoBehaviour
{
    [Header("Cell this door connects")]
    [SerializeField] private Vector2Int cellA;
    [SerializeField] private Vector2Int cellB;

    [Header("Blocked Door?")]
    [SerializeField] private bool isBlocked = false;
    [TextArea(2, 4)]
    [SerializeField] private string blockedDialogue = "That way is blocked.";

    [Header("Audio")]
    [SerializeField] private AudioClip[] openDoorAudios;
    [SerializeField] private AudioClip[] closeDoorAudios;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    [SerializeField] private Animator animator;

    private AudioSource source;

    public bool IsBlocked => isBlocked;
    public string BlockedDialogue => blockedDialogue;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        animator.SetTrigger("Open");
        PlayAudio(openDoorAudios);
    }

    public void CloseDoor()
    {
        animator.SetTrigger("Close");
        PlayAudio(closeDoorAudios);
    }

    public bool Connects(Vector2Int from, Vector2Int to)
    {
        return (cellA == from && cellB == to) ||
            (cellA == to && cellB == from);
    }

    private void PlayAudio(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || source == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip == null) return;

        source.pitch = Random.Range(0.95f, 1.05f);

        source.PlayOneShot(clip, volume);
    }
}
