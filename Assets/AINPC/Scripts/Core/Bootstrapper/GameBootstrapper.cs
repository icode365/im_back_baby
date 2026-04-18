using System.Linq;
using System.Threading.Tasks;
using AINPC.Scripts.Character;
using AINPC.Scripts.Core.AI.Interfaces;
using AINPC.Scripts.Core.Gameplay;
using AINPC.Scripts.Core.Handlers;
using AINPC.Scripts.Data;
using AINPC.Scripts.UI;
using UnityEngine;

namespace AINPC.Scripts.Core.Bootstrapper
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private NpcConversationHandler npcConversationHandler;
        [SerializeField] private UIEventHandler uiEventHandler;
        [SerializeField] private PersonalityHandler personaHandler;
        [SerializeField] private NpcConversationUiHandler conversationUiHandler;
        [SerializeField] private GameplayManager gameplayManager;
        
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
            
            gameplayManager.Init(serviceFactory.GetValidator());

            _service = serviceFactory.InitializeTTSService();

            uiEventHandler.SendButtonOnClick += SendPrompt;
            uiEventHandler.OnPersonaDropdownChanged += ChangeCurrentPersona;
            uiEventHandler.ClearPromptOnClick += conversationUiHandler.ClearPromptInputField;
            
            uiEventHandler.PopulatePersonalityDropdown(personaHandler.GetAvailablePersonaNames());
        }

        private void ChangeCurrentPersona(string name)
        {
            personaHandler.SetCurrentPersona(name);
        }

        // TODO : Check if Fire&Forget is the optimal way here?
        private void SendPrompt()
        {
            _ = SendPromptAsync();
        }

        private async Task SendPromptAsync()
        {
            var userPrompt = GetUserPrompt();
            var systemInstruction = GetSystemInstruction();
            
            var npcResponse = await npcConversationHandler.SendPrompt(userPrompt, systemInstruction);

            // TODO: Show in front-end
            Debug.Log($"Response : {npcResponse.response}");
            
            GlobalEventHandler.Instance.OnApiResponseRecieved(npcResponse);
            
            // var audioResponse = await _service.RequestAudioFor(npcResponse.response);
            //
            // Debug.Log($"Response : {audioResponse.status} | {audioResponse.response}");
            //
            // if (npcResponse.status == EAPIStatus.Success)
            // {
            //     audioSource.clip = npcResponse.responseObject as AudioClip;
            //     audioSource.Play();
            // }
            // else
            // {
            //     Debug.LogError($"Audio playback failed: {npcResponse.error}");
            // }
        }

        private string GetSystemInstruction()
        {
            var systemInstruction = personaHandler.BuildPersonaPrompt();
            var puzzleData = gameplayManager.GetPuzzleData();
            var ingredientData = gameplayManager.GetIngredientData();
            
            systemInstruction += AiPromptBuilder.BuildPrompt(
                puzzleData.puzzleName,
                puzzleData.description,
                ingredientData);
            
            return systemInstruction;
            
        }

        private string GetUserPrompt()
        {
            return uiEventHandler.GetInputFieldText();
        }
    }
}