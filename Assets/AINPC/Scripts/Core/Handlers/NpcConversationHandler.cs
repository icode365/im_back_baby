using System;
using System.Threading.Tasks;
using AINPC.Scripts.Core.AI.Interfaces;
using AINPC.Scripts.Data;
using UnityEngine;

namespace AINPC.Scripts.Core.Handlers
{
    public class NpcConversationHandler : MonoBehaviour
    {
        private ILLMService _llmService = null;

        private void Start()
        {
        }

        public void Initialize(ILLMService llmService)
        {
            _llmService = llmService;
        }

        public async Task<APIResult> SendPrompt(string prompt)
        {
            APIResult apiResult = new();

            if (_llmService == null)
            {
                Debug.LogError($"LLM Service is NULL.");

                // TODO : How can we set the object variables and return instead of duplication of the following 2 lines 
                apiResult.Error = "LLM Service is NULL.";
                apiResult.Status = EAPIStatus.Error;

                return apiResult;
            }

            if (string.IsNullOrEmpty(prompt))
            {
                Debug.LogError($"User Prompt is NULL or empty.");

                apiResult.Error = "User Prompt is NULL or empty.";
                apiResult.Status = EAPIStatus.Error;

                return apiResult;
            }

            try
            {
                Debug.Log("Input Message : " + prompt);
                apiResult = await _llmService.GetResponseAsync(prompt);

                Debug.Log("Response : " + apiResult.Response);
                return apiResult;
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                apiResult.Error = e.Message;
                apiResult.Status = EAPIStatus.Error;

                return apiResult;
                throw;
            }
        }
    }
}