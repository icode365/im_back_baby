using System;
using AINPC.Scripts.Data;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay
{
    public class GlobalEventHandler
    {
        private static GlobalEventHandler _instance;
        public static GlobalEventHandler Instance => _instance ??= new GlobalEventHandler();
        private GlobalEventHandler()
        {
            Debug.Log("[GLOBAL EVENT HANDLER] Created.");
        }
        
        public event Action<ApiResponse> ApiResponseRecieved;

        
        
        public void OnApiResponseRecieved(ApiResponse response)
        {
            ApiResponseRecieved?.Invoke(response);
        }
    }
}