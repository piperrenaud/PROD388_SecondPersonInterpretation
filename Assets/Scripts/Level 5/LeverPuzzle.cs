using UnityEngine;
using UnityEngine.Events;

public class LeverPuzzle : MonoBehaviour
{
    [Header("Puzzle Setting")]
    [SerializeField] public LeverSwitch[] levers;
    [SerializeField] public bool[] requiredPattern;

    [Header("Events")]
    [SerializeField] public UnityEvent onPuzzleSolved;
    [SerializeField] public UnityEvent onPuzzleUnsolved;

    private bool isSolved;

    private void Start()
    {
        CheckPuzzle();
    }

    public void OnLeverFlipped()
    {
        CheckPuzzle();
    }

    private void CheckPuzzle()
    {
        if (levers == null || requiredPattern == null) return;

        if (levers.Length != requiredPattern.Length) return;

        bool solved = true;

        //check every lever against pattern
        for (int i = 0; i < levers.Length; i++)
        {
            if (levers[i] == null)
            {
                solved = false;
                break;
            }

            if (levers[i].IsOn != requiredPattern[i])
            {
                solved = false;
                break;
            }
        }

        //puzzle solved
        if (solved && !isSolved)
        {
            isSolved = true;
            Debug.Log("Lever puzzle solved");
            onPuzzleSolved.Invoke();
        }

        //puzzle solved but then changed
        else if (!solved && isSolved)
        {
            isSolved = false;
            Debug.Log("lever puzzle unsolved");
            onPuzzleUnsolved.Invoke();
        }
    }

    public void TestingPuzzleDebugs(string textToSay)
    {
        Debug.Log(textToSay);
    }

    public void PuzzleSolved()
    {
        //protag asks question for their levers
    }
}