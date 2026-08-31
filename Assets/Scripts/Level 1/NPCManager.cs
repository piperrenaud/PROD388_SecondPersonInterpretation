using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("NPCs")]
    [SerializeField] private NPCController[] npcs;

    [Header("Target")]
    [SerializeField] private Transform targetObject;

    [Header("Gathering Points")]
    [SerializeField] private Transform[] gatheringPoints;
    [SerializeField] private float reunseOffsetRadius = 1f;

    public void GatherNPCs()
    {
        if (npcs == null || npcs.Length == 0) return;
        if (gatheringPoints == null || gatheringPoints.Length == 0) return;

        for (int i = 0; i < npcs.Length; i++)
        {
            int pointIndex = i % gatheringPoints.Length;
            Transform point = gatheringPoints[pointIndex];

            Vector3 offset = GetOffset(i);

            Vector3 destination = point.position + offset;

            npcs[i].WalktTo(destination, targetObject);

            npcs[i].GetComponent<NPCInteraction>().enabled = false;
        }
    }

    private Vector3 GetOffset(int npcIndex)
    {
        int groupIndex = npcIndex / gatheringPoints.Length;

        if (groupIndex == 0) return Vector3.zero;

        float angle = (groupIndex - 1) * 90f;

        Vector3 offset =
            Quaternion.Euler(0f, angle, 0f)
            * Vector3.forward
            * reunseOffsetRadius
            * groupIndex;

        return offset;
    }
}
