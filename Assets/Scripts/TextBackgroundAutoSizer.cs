using UnityEngine;
using TMPro;

public class TextBackgroundAutoSizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float horizontalPadding = 40f;
    [SerializeField] private float verticalPadding = 30f;
    [SerializeField] private float maxWidth = 750f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        ResizeBackground();
    }

    private void ResizeBackground()
    {
        if (dialogueText == null) return;

        dialogueText.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            maxWidth);

        dialogueText.ForceMeshUpdate();

        Vector2 preferredSize = dialogueText.GetPreferredValues(
            dialogueText.text,
            maxWidth,
            0);

        float width = Mathf.Min(
            preferredSize.x + horizontalPadding,
            maxWidth + horizontalPadding);

        float height = preferredSize.y + verticalPadding;

        rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            width);

        rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            height);
    }


}
