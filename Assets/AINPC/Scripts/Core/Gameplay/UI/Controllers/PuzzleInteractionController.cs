using System;
using AINPC.Scripts.Core.Gameplay.UI.Handlers;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay.UI.Controller
{
    public class PuzzleInteractionController : MonoBehaviour
    {
        private IngredientSlot _selectedSlot = null;
        private SelectableRawIngredient _selectedIngredient = null;

        [SerializeField] private PuzzlePanelHandler puzzlePanel = null;

        public event Action<Recipe.Recipe> ValidateOnBrew;

        private void Start()
        {
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            puzzlePanel.BrewButtonClicked += BrewButtonClicked;
            
            puzzlePanel.SlotSelected += HandleSlotSelected;
            puzzlePanel.SlotDeselected += HandleSlotDeselected;

            puzzlePanel.IngredientSelected += HandleRawIngSelected;
            puzzlePanel.IngredientDeselected += HandleIngredientDeselected;
        }

        private void RemoveListeners()
        {
            puzzlePanel.BrewButtonClicked -= BrewButtonClicked;
            
            puzzlePanel.SlotSelected -= HandleSlotSelected;
            puzzlePanel.SlotDeselected -= HandleSlotDeselected;

            puzzlePanel.IngredientSelected -= HandleRawIngSelected;
            puzzlePanel.IngredientDeselected -= HandleIngredientDeselected;
        }
        
        private void HandleRawIngSelected(SelectableRawIngredient newIng)
        {
            if (_selectedIngredient)
            {
                _selectedIngredient.Deselect();
            }

            _selectedIngredient = newIng;

            TryAssignSelectedIngredientToSelectedSlot();
        }

        private void HandleIngredientDeselected(SelectableRawIngredient ingredient)
        {
            _selectedIngredient = null;
        }

        private void HandleSlotDeselected(IngredientSlot deselectedSlot)
        {
            var slot = deselectedSlot;
            if (slot == null)
            {
                return;
            }

            if (_selectedSlot == slot)
            {
                _selectedSlot = null;
            }
        }

        private void HandleSlotSelected(IngredientSlot _selectedSlot)
        {
            var slot = _selectedSlot;

            if (slot == null)
            {
                return;
            }

            Debug.Log("Selected Slot : " + slot.gameObject.GetInstanceID());

            if (IsSlotSelected() && this._selectedSlot != slot)
            {
                this._selectedSlot.Deselect();
            }

            this._selectedSlot = slot;
            Debug.Log("Selected Slot : " + _selectedSlot.gameObject.GetInstanceID());
            TryAssignSelectedIngredientToSelectedSlot();
        }

        private void TryAssignSelectedIngredientToSelectedSlot()
        {
            if (IsSlotSelected() && IsIngSelected())
            {
                AssignSelectedIngToSlot(_selectedIngredient.RawIng);
            }
        }

        private void AssignSelectedIngToSlot(Interfaces.RawIngredient ing)
        {
            Debug.Log("Selected Slot : " + ing.ingredientName);
            _selectedSlot.AssignIngredient(ing);
            _selectedIngredient.Deselect();
            _selectedSlot.Deselect();
            _selectedIngredient = null;
            _selectedSlot = null;
        }


        private bool IsSlotSelected() => _selectedSlot != null;

        private bool IsIngSelected() => _selectedIngredient != null;

        private void BrewButtonClicked()
        {
            var userRecipe = new Recipe.Recipe(puzzlePanel.GetRawIngUserInputList());
            
            ValidateOnBrew?.Invoke(userRecipe);
        }
    }
}