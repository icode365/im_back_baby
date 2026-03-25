using AINPC.Scripts.Core.AI.Interfaces;
using AINPC.Scripts.Core.Handlers;
using AINPC.Scripts.Core.State;
using Unity.VisualScripting;
using UnityEngine;

namespace AINPC.Scripts.Core.Bootstrapper
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private NpcConversationHandler npcConversationHandler;
        [SerializeField] private UIEventHandler uiEventHandler;

        private EGameState gameState = EGameState.Idle;
        
        private ServiceFactory.ServiceFactory serviceFactory = new();

        private void Start()
        {
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            ILLMService service = serviceFactory.InitializeService();
            npcConversationHandler.Initialize(service);
            
            uiEventHandler.
        }
    }
}