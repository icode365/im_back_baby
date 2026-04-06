using System;
using System.Collections.Generic;
using AINPC.Scripts.Core.UI;
using AINPC.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField] public Button PromptSendButton;
    [SerializeField] public InputField PromptInputField;

    [SerializeField] private PersonalityDropdownHandler personalityDropdownHandler;

    public event Action SendButtonOnClick;
    public event Action<string> OnPersonaDropdownChanged;

    private void Start()
    {
        if (PromptSendButton == null)
        {
            Debug.LogError("sendButton is null.");
            return;
        }
        
        PromptSendButton.onClick.AddListener(() => SendButtonOnClick?.Invoke());
        personalityDropdownHandler.onPersonaChanged += OnPersonaDropdownChanged;
    }

    public void PopulatePersonalityDropdown(List<string> personasNames)
    {
        personalityDropdownHandler.Populate(personasNames);
    }
}