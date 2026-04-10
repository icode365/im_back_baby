using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;
using AINPC.Scripts.Core.Gameplay.UI;
using AINPC.Scripts.Core.Gameplay.UI.Base;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        public RawIngredients ingredients = null;

        public RawIngredientUiController rawIngUiController = null;
        public GameObject ingHorizontalContainer = null;
        public RawIngredientSlot ingSlot = null;
        public Transform slotParent = null;

        private RawIngredientUiController selectedIng = null;
        private RawIngredientSlot selectedSlot = null;

        private void Start()
        {
            ingredients.rawIngredients.ForEach(ing => SpawnRawIngredients(ing));
        }

        private void SpawnRawIngSlots()
        {
            var newIngSlot = Instantiate(ingSlot, slotParent);
            newIngSlot.Selected += HandleSlotSelected;
            newIngSlot.Deselected += HandleSlotDeselected;
        }

        private void HandleSlotSelected(SelectableUi<RawIngredientSlot> selectedSlot)
        {
            var slot = selectedSlot as RawIngredientSlot;
            if (slot == null)
            {
                return;
            }

            if (selectedSlot != null && selectedSlot != slot)
            {
                selectedSlot.Deselect();
            }

            selectedSlot = slot;
            PostSelectionAssignment();
        }

        private void HandleSlotDeselected(SelectableUi<RawIngredientSlot> deselectedSlot)
        {
            var slot = deselectedSlot as RawIngredientSlot;
            if (slot == null)
            {
                return;
            }

            if (selectedSlot == slot)
            {
                selectedSlot = null;
            }
        }


        private void SpawnRawIngredients(RawIngredient ing)
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

            PostSelectionAssignment();
        }

        private void PostSelectionAssignment()
        {
            if (IsSlotSelected() && IsIngSelected())
            {
                AssignSelectedIngToSlot(selectedIng.RawIng);
            }
        }

        private bool IsSlotSelected()
        {
            return selectedSlot != null;
        }

        private bool IsIngSelected() => selectedIng != null;
        
        private void AssignSelectedIngToSlot(RawIngredient ing)
        {
            selectedSlot.AssignIngredient(ing);
        }
    }
}