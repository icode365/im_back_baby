using AINPC.Scripts.AI;
using AINPC.Scripts.Core.AI.Interfaces;
using UnityEngine;

namespace AINPC.Scripts.Core.ServiceFactory
{
    public class ServiceFactory
    {
        public ILLMService InitializeService()
        {
            return new GeminiAIService();
        }
    }
}