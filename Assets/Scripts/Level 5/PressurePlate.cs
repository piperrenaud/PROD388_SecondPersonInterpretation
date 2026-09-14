using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Button Movement")]
    public Transform buttonTop;
    public ButtonGroup buttonGroup;

    private float pressDepth = 0.3f;
    private float pressSpeed = 6f;

    private Vector3 startPos;
    private Vector3 pressedPos;
    private bool isPressed;
    private int playerCount = 0;

    public bool IsPressed => isPressed;

    private void Start()
    {
        startPos = buttonTop.localPosition;
        pressedPos = startPos + Vector3.down * pressDepth;

        buttonGroup = GetComponentInParent<ButtonGroup>();
    }

    private void Update()
    {
        Vector3 target = playerCount > 0 ? pressedPos : startPos;

        buttonTop.localPosition = Vector3.Lerp(
            buttonTop.localPosition,
            target,
            Time.deltaTime * pressSpeed
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerCount++;

        if (!isPressed)
        {
            isPressed = true;
            buttonGroup.CheckButtons();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerCount--;

        if (playerCount <= 0)
        {
            isPressed = false;
            buttonGroup.CheckButtons();
        }
    }
}
