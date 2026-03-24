using System;
using AINPC.Scripts.AI;
using AINPC.Scripts.Core.AI.Interfaces;
using UnityEngine;

namespace AINPC.Scripts.Core.Handlers
{
    public class NpcConversationHandler : MonoBehaviour
    {
        [SerializeField] private UIEventHandler _uiEventHandler = null;

        private ILLMService _llmService = null;

        private void Start()
        {
            _uiEventHandler.OnSendEvent.AddListener((m) => SendMessage(m));

            _llmService = new GeminiAIService();
        }

        private async void SendMessage(string message)
        {
            try
            {
                Debug.Log("Input Message : " + message);
                var response = await _llmService.GetResponseAsync(message);

                Debug.Log("Response : " + response);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                throw;
            }
        }
    }
}