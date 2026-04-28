using System;
using AINPC.Scripts.Core.Gameplay.Data;
using AINPC.Scripts.Core.Gameplay.UI.Handlers;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay.UI.Controller
{
    public class PuzzleInteractionController : MonoBehaviour
    {
        private IngredientSlot _selectedSlot = null;
        private SelectableRawIngredient _selectedIngredient = null;

        [SerializeField] private Transform snackbar;

        [SerializeField] private PuzzlePanelEventHandler puzzlePanelEventHandler = null;

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
            puzzlePanelEventHandler.BrewButtonClicked += BrewButtonClicked;

            puzzlePanelEventHandler.SlotSelected += HandleSlotSelected;
            puzzlePanelEventHandler.SlotDeselected += HandleSlotDeselected;

            puzzlePanelEventHandler.IngredientSelected += HandleRawIngSelected;
            puzzlePanelEventHandler.IngredientDeselected += HandleIngredientDeselected;
        }

        private void RemoveListeners()
        {
            puzzlePanelEventHandler.BrewButtonClicked -= BrewButtonClicked;

            puzzlePanelEventHandler.SlotSelected -= HandleSlotSelected;
            puzzlePanelEventHandler.SlotDeselected -= HandleSlotDeselected;

            puzzlePanelEventHandler.IngredientSelected -= HandleRawIngSelected;
            puzzlePanelEventHandler.IngredientDeselected -= HandleIngredientDeselected;
        }

        public void ResetAll()
        {
            _selectedIngredient?.Deselect();
            _selectedIngredient = null;
            _selectedSlot?.Deselect();
            _selectedSlot = null;
            puzzlePanelEventHandler.ResetPuzzle();
        }

        public void LoadPuzzle(PuzzleData data)
        {
            puzzlePanelEventHandler.SetupPuzzleData(data);
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
            var userRecipe = new Recipe.Recipe(puzzlePanelEventHandler.GetRawIngUserInputList());

            ValidateOnBrew?.Invoke(userRecipe);
        }
    }
}