using System;
using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Data;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI.Controller
{
    public class PuzzleUiController : MonoBehaviour
    {
        public Button brew_Btn = null;
        public TMP_Text puzzleName_Txt = null;
        public TMP_Text puzzleDescription_Txt = null;

        public Transform ingHorizontalContainer = null;
        public RawIngredientUiController rawIngUiController_Pf = null;

        public Transform slotsHorizontalContainer = null;
        public RawIngredientSlotUi ingSlotUi_Pf = null;
        [SerializeField] private List<RawIngredientSlotUi> slotsUi = new();

        private RawIngredientSlotUi _selectedSlotUi = null;
        private RawIngredientUiController _selectedIng = null;

        public event Action BrewButtonClicked;

        private void Start()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            brew_Btn.onClick.AddListener(() => BrewButtonClicked?.Invoke());
        }

        public void SetupPuzzleData(PuzzleData puzzleData)
        {
            puzzleName_Txt.text = puzzleData.puzzleName;
            puzzleDescription_Txt.text = puzzleData.description;
        }

        // TODO : This might not be right because this still knows about RawIngredients class which should not be exposed to UIClasses
        public List<RawIngredient> GetRawIngUserInput()
        {
            var userPickedIng = new List<RawIngredient>();
            slotsUi.ForEach(i => userPickedIng.Add(i.AssignedIngredient));
            return userPickedIng;
        }

        public void SpawnRawIngSlot()
        {
            var newIngSlot = Instantiate(ingSlotUi_Pf, slotsHorizontalContainer);
            newIngSlot.Selected += HandleSlotSelected;
            newIngSlot.Deselected += HandleSlotDeselected;
            slotsUi.Add(newIngSlot);
        }

        private void HandleSlotSelected(SelectableUi<RawIngredientSlotUi> _selectedSlot)
        {
            var slot = _selectedSlot as RawIngredientSlotUi;

            Debug.Log("Selected Slot : " + slot.gameObject.GetInstanceID());
            if (slot == null)
            {
                return;
            }

            if (IsSlotSelected() && this._selectedSlotUi != slot)
            {
                this._selectedSlotUi.Deselect();
            }

            this._selectedSlotUi = slot;
            Debug.Log("Selected Slot : " + _selectedSlot.gameObject.GetInstanceID());
            PostSelectionAssignment();
        }

        private void HandleSlotDeselected(SelectableUi<RawIngredientSlotUi> deselectedSlot)
        {
            var slot = deselectedSlot as RawIngredientSlotUi;
            if (slot == null)
            {
                return;
            }

            if (_selectedSlotUi == slot)
            {
                _selectedSlotUi = null;
            }
        }


        private bool IsSlotSelected() => _selectedSlotUi != null;

        private bool IsIngSelected() => _selectedIng != null;

        public void SpawnRawIngredients(RawIngredient ing)
        {
            var ingUi = Instantiate(rawIngUiController_Pf, ingHorizontalContainer);
            ingUi.SetupRawIngUI(ing);
            ingUi.Selected += HandleRawIngSelected;
            ingUi.Deselected += (m) => _selectedIng = null;
        }

        private void HandleRawIngSelected(SelectableUi<RawIngredientUiController> newIng)
        {
            if (_selectedIng)
            {
                _selectedIng.Deselect();
            }

            _selectedIng = newIng as RawIngredientUiController;

            PostSelectionAssignment();
        }

        private void PostSelectionAssignment()
        {
            if (IsSlotSelected() && IsIngSelected())
            {
                AssignSelectedIngToSlot(_selectedIng.RawIng);
            }
        }
        
        private void AssignSelectedIngToSlot(RawIngredient ing)
        {
            Debug.Log("Selected Slot : " + ing.ingredientName);
            _selectedSlotUi.AssignIngredient(ing);
            _selectedIng.Deselect();
            _selectedSlotUi.Deselect();
            _selectedIng = null;
            _selectedSlotUi = null;
        }
    }
}