using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField] public Button PromptSendButton;
    [SerializeField] public InputField PromptInputField;

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