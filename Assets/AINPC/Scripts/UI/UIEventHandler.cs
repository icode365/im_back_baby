using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIEventHandler : MonoBehaviour
{
    [feild: SerializeField] public Button PromptSendButton { get; private set; }
    [feild: SerializeField] public InputField PromptInputField { get; private set; }

    public event Action SendButtonOnClick;

    private void Start()
    {
        if (PromptSendButton == null)
        {
            Debug.LogError("sendButton is null.");
            return;
        }
        
        PromptSendButton.onClick.AddListener(() => SendButtonOnClick?.Invoke());
    }
}