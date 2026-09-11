using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("All Objectives")]
    [SerializeField] private ObjectiveArea[] objectives;
    [SerializeField] private int currentObjectiveIndex = -1;

    public int CurrentObjectiveIndex => currentObjectiveIndex;

    private void Start()
    {
        Instance = this;
        StartNextObjective();
    }

    private void StartNextObjective()
    {
        currentObjectiveIndex++;

        objectives[currentObjectiveIndex].SetObjectiveActive(true);
        objectives[currentObjectiveIndex].SetOutline(true);
    }

    public void CompleteCurrentObjective()
    {
        objectives[currentObjectiveIndex].SetOutline(false);
        objectives[currentObjectiveIndex].SetObjectiveActive(false);

        StartNextObjective();
    }

    public ObjectiveArea CurrentObjective()
    {
        if (currentObjectiveIndex < 0 || currentObjectiveIndex >= objectives.Length)
        {
            return null;
        }

        return objectives[currentObjectiveIndex];
    }
}
