using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("Movement")]
    public bool hasMoved;
    public bool isMoving;

    [Header("Interactions")]
    public int totalInteractions;

    [Header("Doors")]
    public int doorsOpened;
    public int doorsClosed;

    [Header("Lights")]
    public int lightsTurnedOn;
    public int lightsTurnedOff;
}
