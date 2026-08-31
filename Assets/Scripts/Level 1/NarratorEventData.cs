using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NarratorEvent", menuName = "Narrator/Event")]
public class NarratorEventData : ScriptableObject
{
    [Header("Event")]
    public string eventID;

    [Header("Dialogue")]
    [TextArea(3, 10)]
    public List<string> dialogueLines = new List<string>();

    [Header("Settings")]
    public bool canRepeat;

    [Tooltip("Higher prioty events interrupt lower priority events")]
    public int priority = 0;
}