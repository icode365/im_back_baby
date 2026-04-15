using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Data;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;
using AINPC.Scripts.Core.Gameplay.UI;
using AINPC.Scripts.Core.Gameplay.UI.Base;
using AINPC.Scripts.Core.Gameplay.UI.Controller;
using AINPC.Scripts.Core.Gameplay.Validator;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        private IValidator validator;
        public PuzzleData _puzzleData;
        public RawIngredientsData ingredientsData = null;

        [SerializeField] private PuzzleUiController puzzleUi;

        public void Init(IValidator _validator)
        {
            validator = _validator;
        }

        private void Start()
        {
            ingredientsData.rawIngredients.ForEach(ing => puzzleUi.SpawnRawIngredients(ing));
            _puzzleData.rawIngredients.ForEach(ing => puzzleUi.SpawnRawIngSlot());

            puzzleUi.BrewButtonClicked += BrewButtonClicked;
            puzzleUi.SetupPuzzleData(_puzzleData);
        }

        private void BrewButtonClicked()
        {
            IRecipe userRecipe = new Recipe(puzzleUi.GetRawIngUserInput());
            IRecipe puzzleRecipe = new Recipe(_puzzleData.rawIngredients);
            
            var validationResult = validator.Validate(puzzleRecipe, userRecipe);
            
            OnValidationCompleted(validationResult);
        }

        private void OnValidationCompleted(ValidationResult result)
        {
            Debug.Log($"Result : {result.Correct},\n Result.PartiallyCorrect : {result.PartiallyCorrect}");
        }
        
        
    }
}