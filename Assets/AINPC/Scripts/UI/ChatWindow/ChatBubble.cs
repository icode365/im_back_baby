using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] private RectTransform bubbleRect;
    [SerializeField] private TMP_Text bubbleText;
    [SerializeField] private float maxWidth = 350f;
    [SerializeField] private float paddingX = 5f;
    [SerializeField] private float paddingY = 5f;
    
    void Start()
    {
        if(!bubbleRect) Debug.LogError($"[{this.name}] Bubble Rect is null.");
        if(!bubbleText) Debug.LogError($"[{this.name}] Bubble Text is null.");
    }

    public void Initialize(string text)
    {
        SetText(text);
        ResizeBubble();
    }
    
    private void ResizeBubble()
    {
        if (bubbleRect == null || bubbleText == null)
        {
            return;
        }

        bubbleText.textWrappingMode = TextWrappingModes.Normal;

        // Measure preferred size for the current text
        Vector2 preferredSize = bubbleText.GetPreferredValues(
            bubbleText.text,
            maxWidth - paddingX,
            0f
        );

        float targetWidth = Mathf.Min(preferredSize.x + paddingX, maxWidth);
        float targetHeight = preferredSize.y + paddingY;

        bubbleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
        bubbleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
    }

    private void SetText(string text)
    {
        bubbleText.text = text;
    }
}
