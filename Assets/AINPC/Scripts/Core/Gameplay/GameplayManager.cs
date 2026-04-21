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
        [SerializeField] private PuzzleData puzzleData; // TODO : this should be list of puzzle Levels or it should be one scriptableObject with list of levels
        [SerializeField] private IngredientsData ingredientsData = null;
        [SerializeField] private PuzzlePanelHandler puzzlePanelHandler = null;
        [SerializeField] private PuzzleInteractionController puzzleInteractionController = null;

        public void Initialize(IValidator validator)
        {
            _validator = validator;
        }

        private void Start()
        {
            puzzlePanelHandler.Initialize(puzzleData, ingredientsData);

            puzzleInteractionController.ValidateOnBrew += BrewButtonClicked;
        }

        public PuzzleData GetPuzzleData() => puzzleData;
        public IngredientsData GetIngredientData() => ingredientsData;
        
        private void BrewButtonClicked(Recipe.Recipe userRecipe)
        {
            IRecipe puzzleRecipe = new Recipe.Recipe(puzzleData.rawIngredients);
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
        }
    }
}