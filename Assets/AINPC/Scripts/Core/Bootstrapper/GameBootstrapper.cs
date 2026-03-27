using System.Threading.Tasks;
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
            
            uiEventHandler.SendButtonOnClick += SendPrompt ;
        }

        // TODO : Check if Fire&Forget is the optimal way here?
        private void SendPrompt()
        {
            _ = SendPromptAsync();
        }
        
        private async Task SendPromptAsync()
        {
            var userPrompt = GetUserPrompt();
            var npcResponse = await npcConversationHandler.SendPrompt(userPrompt);
            
            // TODO: Show in front-end
            Debug.Log($"Response : {npcResponse.Response}");
        }

        private string GetUserPrompt()
        {
            return uiEventHandler.PromptInputField.text;
        }
    }
}