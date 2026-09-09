using UnityEngine;
using UnityEngine.UI;

public class DoorButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MehcnaicalDoors door;
    [SerializeField] private Image buttonImage;

    [Header("Colours")]
    [SerializeField] private Color openColour = Color.green;
    [SerializeField] private Color closeColour = Color.red;

    private void Start()
    {
        UpdateColour();
    }

    private void Update()
    {
        UpdateColour();
    }

    public void ToggleDoor()
    {
        door.ToggleDoor();
    }

    private void UpdateColour()
    {
        if (door == null || buttonImage == null) return;

        buttonImage.color = door.IsOpen ? openColour : closeColour;
    }
}
