using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;
using AINPC.Scripts.Core.Gameplay.UI;
using AINPC.Scripts.Core.Gameplay.UI.Base;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        // Start from Introduction panel
        // Show welcome screen
        // Introduction of Goal
        // Intro of Conflict
        // Intro of NPCs their limitations and constraints
        // Start Gameplay
        // Start the puzzle
        // Show the NPC options
        // User selects which NPC abilities to use
        // User enters prompts to get the correct answers
        // User enters the answer in the puzzle to open solve the puzzle

        public RawIngredients ingredients = null;
        public RawIngredientUiController rawIngUiController = null;
        public GameObject ingHorizontalContainer = null;

        private RawIngredientUiController selectedIng = null;
        private RawIngredientSlot selectedSlot = null;
        
        private void Start()
        {
            ingredients.rawIngredients.ForEach(ing => SpawnRawIngSlot(ing));
        }

        private void SpawnRawIngSlot(RawIngredient ing)
        {
            var ingUi = Instantiate(rawIngUiController, ingHorizontalContainer.transform);
            ingUi.SetupRawIngUI(ing);
            ingUi.Selected += HandleRawIngSelected;
            ingUi.Deselected += (m) => selectedIng = null;
        }

        private void HandleRawIngSelected(SelectableUi<RawIngredientUiController> newIng)
        {
            if (selectedIng)
            {
                selectedIng.Deselect();
            }

            selectedIng = newIng as RawIngredientUiController;

            if (IsSlotSelected() && selectedIng)
            {
                AssignSelectedIngToSlot(selectedIng.RawIng);
            }
        }

        private bool IsSlotSelected()
        {
            return selectedSlot != null;
        }

        private void AssignSelectedIngToSlot(RawIngredient ing)
        {
            selectedSlot.AssignIngredient(ing);
        }
    }
}