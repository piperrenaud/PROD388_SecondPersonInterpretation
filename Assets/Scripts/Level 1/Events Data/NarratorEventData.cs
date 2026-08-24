using UnityEngine;

[CreateAssetMenu(fileName = "NarratorEvent", menuName = "Narrator/Event")]
public class NarratorEventData : ScriptableObject
{
    [Header("Event")]
    public string eventID;

    [Header("Dialogue")]
    [TextArea(3, 10)]
    public string dialogue;

    [Header("Settings")]
    public bool canRepeat;
}