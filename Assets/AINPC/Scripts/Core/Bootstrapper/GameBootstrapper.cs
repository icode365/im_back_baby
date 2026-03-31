using System.Threading.Tasks;
using AINPC.Scripts.Core.AI.Interfaces;
using AINPC.Scripts.Core.Handlers;
using AINPC.Scripts.Core.State;
using AINPC.Scripts.Data;
using UnityEngine;

namespace AINPC.Scripts.Core.Bootstrapper
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private NpcConversationHandler npcConversationHandler;
        [SerializeField] private UIEventHandler uiEventHandler;

        private EGameState gameState = EGameState.Idle;

        // TODO : Just for testing, remove from here
        private ITtsService _service = null;
        public AudioSource audioSource = null;

        private ServiceFactory.ServiceFactory serviceFactory = null;

        private void Start()
        {
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            serviceFactory = new();
            ILLMService service = serviceFactory.InitializeLlmService();
            npcConversationHandler.Initialize(service);

            _service = serviceFactory.InitializeTTSService();

            uiEventHandler.SendButtonOnClick += SendPrompt;
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
            Debug.Log($"Response : {npcResponse.response}");
            var audioResponse = await _service.RequestAudioFor(npcResponse.response);

            Debug.Log($"Response : {audioResponse.status} | {audioResponse.response}");

            if (audioResponse.status == EAPIStatus.Success && audioResponse.responseObject is AudioClip clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError($"Audio playback failed: {audioResponse.error}");
            }
        }

        private string GetUserPrompt()
        {
            return uiEventHandler.PromptInputField.text;
        }
    }
}