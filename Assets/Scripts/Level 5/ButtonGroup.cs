using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Events;

public class ButtonGroup : MonoBehaviour
{
    [Header("Buttons in group")]
    public PressurePlate[] requiredButtons;

    [Header("Button Behaviour")]
    public bool buttonsMustStayPressed = true;

    [Header("Event when pressed")]
    public UnityEvent onAllButtonsPressed;
    public UnityEvent onButtonsReleased;

    private bool eventActive;
    private bool permanentlyActivated;

    public void CheckButtons()
    {
        if (permanentlyActivated) return;

        bool allPressed = true;

        foreach (PressurePlate button in requiredButtons)
        {
            if (!button.IsPressed)
            {
                allPressed = false;
                break;
            }
        }

        if (allPressed)
        {
            if (!eventActive)
            {
                eventActive = true;
                onAllButtonsPressed.Invoke();

                if (!buttonsMustStayPressed)
                {
                    permanentlyActivated = true;
                }
            }
        }
        else
        {
            if (eventActive && buttonsMustStayPressed)
            {
                eventActive = false;
                onButtonsReleased.Invoke();
            }
        }
    }
}
