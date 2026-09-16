using UnityEngine;

public class LeverSwitch : MonoBehaviour
{
    [Header("Lever Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool startsOn = false;
    [SerializeField] private float coolDown = 0.5f;

    [Header("Puzzle")]
    [SerializeField] private LeverPuzzle puzzle;

    [Header("Audio")]
    [SerializeField][Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private AudioClip[] clips;

    private AudioSource source;

    public bool IsOn { get; private set; }

    private float lastFlipTime = -999f;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        IsOn = startsOn;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (IsOn)
        {
            animator.SetTrigger("Off");
        }
        else
        {
            animator.SetTrigger("On");
        }
    }

    public void ToggleLever()
    {
        if (Time.time < lastFlipTime + coolDown) return;

        Flip();

        PlaySound();
    }

    private void Flip()
    {
        lastFlipTime = Time.time;

        IsOn = !IsOn;

        if (IsOn) animator.SetTrigger("Off");
        else animator.SetTrigger("On");

        if (puzzle != null)
        {
            puzzle.OnLeverFlipped();
        }
    }

    private void PlaySound()
    {
        if (clips == null || clips.Length == 0 || source == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip == null) return;

        source.pitch = Random.Range(0.95f, 1.05f);
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 1.5f;
        source.maxDistance = 12f;
        source.dopplerLevel = 0f;

        source.PlayOneShot(clip, volume);
    }
}