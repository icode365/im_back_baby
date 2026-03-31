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

        public void Initialize(ILLMService llmService)
        {
            _llmService = llmService;
        }

        public async Task<ApiResponse> SendPrompt(string prompt, string systemInfo)
        {
            ApiResponse apiResponse = new();

            if (_llmService == null)
            {
                Debug.LogError($"LLM Service is NULL.");

                // TODO : How can we set the object variables and return instead of duplication of the following 2 lines 
                apiResponse.error = "LLM Service is NULL.";
                apiResponse.status = EAPIStatus.Error;

                return apiResponse;
            }

            if (string.IsNullOrEmpty(prompt))
            {
                Debug.LogError($"User Prompt is NULL or empty.");

                apiResponse.error = "User Prompt is NULL or empty.";
                apiResponse.status = EAPIStatus.Error;

                return apiResponse;
            }

            try
            {
                Debug.Log("Input Message : " + prompt);
                apiResponse = await _llmService.GetResponseAsync(prompt, systemInfo);

                Debug.Log("Response : " + apiResponse.response);
                return apiResponse;
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                apiResponse.error = e.Message;
                apiResponse.status = EAPIStatus.Error;

                return apiResponse;
                throw;
            }
        }
    }
}