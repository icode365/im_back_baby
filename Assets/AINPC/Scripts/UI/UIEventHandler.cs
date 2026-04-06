using System;
using System.Collections.Generic;
using AINPC.Scripts.Core.UI;
using AINPC.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField] private Button PromptSendButton;
    [SerializeField] private InputField PromptInputField;
    [SerializeField] private Button clearPromptButton;

    [SerializeField] private PersonalityDropdownHandler personalityDropdownHandler;

    public event Action SendButtonOnClick;
    public event Action<string> OnPersonaDropdownChanged;
    public event Action ClearPromptOnClick;

    private void Start()
    {
        if (PromptSendButton == null)
        {
            Debug.LogError("sendButton is null.");
            return;
        }

        PromptSendButton.onClick.AddListener(() => SendButtonOnClick?.Invoke());
        personalityDropdownHandler.onPersonaChanged += OnPersonaDropdownChanged;
        
        clearPromptButton.onClick.AddListener(() => ClearPromptOnClick?.Invoke());
    }

    public void PopulatePersonalityDropdown(List<string> personasNames)
    {
        personalityDropdownHandler.Populate(personasNames);
    }

    public string GetInputFieldText() => PromptInputField.text;
}