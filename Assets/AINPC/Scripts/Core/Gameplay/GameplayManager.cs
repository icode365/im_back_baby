using AINPC.Scripts.Core.Gameplay.Data;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;
using AINPC.Scripts.Core.Gameplay.UI.Controller;
using AINPC.Scripts.Core.Gameplay.UI.Handlers;
using AINPC.Scripts.Core.Gameplay.Validator;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        private IValidator _validator;
        [SerializeField] private PuzzleData puzzleData; // TODO : this should be list of puzzle Levels or it should be one scriptableObject with list of levels
        [SerializeField] private IngredientsData ingredientsData = null;
        [SerializeField] private PuzzlePanelHandler puzzlePanelHandler = null;
        [SerializeField] private PuzzleInteractionController puzzleInteractionController = null;

        public void Init(IValidator validator)
        {
            _validator = validator;
        }

        private void Start()
        {
            puzzlePanelHandler.Initialize(puzzleData, ingredientsData);

            puzzleInteractionController.ValidateOnBrew += BrewButtonClicked;
        }

        private void BrewButtonClicked(Recipe.Recipe userRecipe)
        {
            IRecipe puzzleRecipe = new Recipe.Recipe(puzzleData.rawIngredients);
            var validationResult = _validator.Validate(puzzleRecipe, userRecipe);
            
            OnValidationCompleted(validationResult);
        }

        private void OnValidationCompleted(ValidationResult result)
        {
            Debug.Log($"Result : {result.Correct},\n Result.PartiallyCorrect : {result.PartiallyCorrect}");
        }
    }
}