using UnityEngine;

public class LeverSwitch : MonoBehaviour
{
    [Header("Lever Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool startsOn = false;
    [SerializeField] private float coolDown = 0.5f;

    [Header("Puzzle")]
    [SerializeField] private LeverPuzzle puzzle;

    public bool IsOn { get; private set; }

    private float lastFlipTime = -999f;

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
}