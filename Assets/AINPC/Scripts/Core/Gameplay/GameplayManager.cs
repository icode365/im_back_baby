using System.Collections.Generic;
using System.Linq;
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
        private int _currentPuzzleIndex = 0;

        [SerializeField]
        private List<PuzzleData>
            puzzleData =
                new(); // TODO : this should be list of puzzle Levels or it should be one scriptableObject with list of levels

        [SerializeField] private IngredientsData ingredientsData;
        [SerializeField] private PuzzlePanelEventHandler puzzlePanelEventHandler;
        [SerializeField] private PuzzleInteractionController puzzleInteractionController;

        public void Initialize(IValidator validator)
        {
            _validator = validator;
        }

        private void Start()
        {
            // todo : use PuzzleInteractionController instead of EventHandler
            puzzlePanelEventHandler.Initialize(puzzleData[_currentPuzzleIndex], ingredientsData);

            puzzleInteractionController.ValidateOnBrew += ValidateRecipe;
        }

        public PuzzleData GetPuzzleData() => puzzleData[_currentPuzzleIndex];
        public IngredientsData GetIngredientData() => ingredientsData;

        private void ValidateRecipe(Recipe.Recipe userRecipe)
        {
            IRecipe puzzleRecipe = new Recipe.Recipe(puzzleData[_currentPuzzleIndex].rawIngredients);
            var validationResult = _validator.Validate(puzzleRecipe, userRecipe);

            RecipeProperties recipeProperties = new();
            List<string> properties = new();

            userRecipe.rawIngredients.ForEach(i => properties.AddRange(GetPropertiesFor(i.ingredientName)));
            recipeProperties.SetProperties(properties);

            OnValidationCompleted(validationResult, recipeProperties);
        }

        private List<string> GetPropertiesFor(string ingredientName)
        {
            var ingredientData = ingredientsData.rawIngredients.First(i => i.ingredientName == ingredientName);
            return ingredientData.properties;
        }

        private void OnValidationCompleted(ValidationResult result, RecipeProperties properties)
        {
            Debug.Log($"Result : {result.Correct},\n Result.PartiallyCorrect : {result.PartiallyCorrect}");
            Debug.Log($"Resultant Properties : {string.Join(",", properties.ResultantProperties)}");

            GlobalEventHandler.Instance.OnBrewed(result, properties);

            if (result.Correct)
            {
                _currentPuzzleIndex++;
                
                if (_currentPuzzleIndex < puzzleData.Count)
                    LoadNextPuzzle(puzzleData[_currentPuzzleIndex]);
            }
        }

        private void LoadNextPuzzle(PuzzleData puzzle)
        {
            puzzleInteractionController.ResetAll();
            puzzleInteractionController.LoadPuzzle(puzzle);
        }
    }
}