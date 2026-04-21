using System;
using AINPC.Scripts.Core.Gameplay.Validator;
using AINPC.Scripts.Data;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay
{
    // todo : maybe refactor in a way where not every script references this 
    public class GlobalEventHandler
    {
        private static GlobalEventHandler _instance;
        public static GlobalEventHandler Instance => _instance ??= new GlobalEventHandler();
        private GlobalEventHandler()
        {
            Debug.Log("[GLOBAL EVENT HANDLER] Created.");
        }
        
        public event Action<ApiResponse> ApiResponseRecieved;
        public event Action<ValidationResult, RecipeProperties> RecipeValidated;

        
        
        public void OnApiResponseRecieved(ApiResponse response)
        {
            ApiResponseRecieved?.Invoke(response);
        }

        public void OnBrewed(ValidationResult result, RecipeProperties properties)
        {
            RecipeValidated?.Invoke(result, properties);
        }
    }
}