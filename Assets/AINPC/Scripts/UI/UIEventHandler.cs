using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField] private Button sendButton = null;
    [SerializeField] private InputField inputField = null;

    public UnityEvent<string> OnSendEvent { get; private set; } = new();
    
    private void Start()
    {
        if (sendButton == null)
        {
            Debug.LogError("sendButton is null.");
        }
        else
        {
            MapEventButton(sendButton, OnSend);
        }
    }

    private void MapEventButton(Button button, Action onInteracted)
    {
        button.onClick.AddListener(() => onInteracted?.Invoke());
    }

    private void OnSend()
    {
        string userInput = inputField.text;
        Debug.Log("Send Clicked");
        OnSendEvent?.Invoke(userInput);
    }
}
