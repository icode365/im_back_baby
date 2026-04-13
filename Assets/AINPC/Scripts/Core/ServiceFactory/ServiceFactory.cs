using AINPC.Scripts.AI;
using AINPC.Scripts.Core.AI.Interfaces;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.Validator;
using AINPC.Scripts.Data;
using UnityEngine;

namespace AINPC.Scripts.Core.ServiceFactory
{
    public class ServiceFactory 
    {
        private AiSetting aiSetting;

        public ServiceFactory() => LoadAISettings();

        private void LoadAISettings()
        {
            aiSetting = Resources.Load("Data/GeminiAICreds") as AiSetting;

            if (!AiSettingLoaded())
            {
                Debug.LogError("Error Gemini API Creds are unavailable.");
            }
        }

        private bool AiSettingLoaded() => aiSetting != null;

        public ILLMService InitializeLlmService()
        {
            if (!AiSettingLoaded())
            {
                Debug.LogError("Error Loading Gemini API Creds");
                return null;
            }

            Gemini_2_5_FlashLite_Service service = new();
            service.Initialize(aiSetting);

            return service;
        }

        public ITtsService InitializeTTSService()
        {
            if (!AiSettingLoaded())
            {
                Debug.LogError("Error Loading Gemini API Creds");
                return null;
            }

            Gemini_2_5_FlashLite_Tts_Service ttsService = new();
            ttsService.Initialize(aiSetting);
            return ttsService;
        }

        public IValidator GetValitor()
        {
            return new SolutionValidator();
        }
    }
}