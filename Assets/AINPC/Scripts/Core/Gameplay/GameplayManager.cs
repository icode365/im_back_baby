using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Data;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;
using AINPC.Scripts.Core.Gameplay.UI;
using AINPC.Scripts.Core.Gameplay.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        private IValidator validator;
        public RawIngredients ingredients = null;

        public RawIngredientUiController rawIngUiController = null;
        public GameObject ingHorizontalContainer = null;
        public RawIngredientSlot ingSlot = null;
        public Transform slotParent = null;

        public Button brew_Btn = null;
        public TMP_Text puzzle_Txt = null;
        public TMP_Text puzzle_Desc = null;
        
        public PuzzleData _puzzleData;
        private RawIngredientSlot selectedSlot = null;
        [SerializeField] private List<RawIngredientSlot> slots = new();
        private RawIngredientUiController selectedIng = null;

        public void Init(IValidator _validator)
        {
            validator = _validator;
        }
        
        private void Start()
        {
            ingredients.rawIngredients.ForEach(ing => SpawnRawIngredients(ing));
            _puzzleData.rawIngredients.ForEach(ing => SpawnRawIngSlot());
            brew_Btn.onClick.AddListener(BrewButtonClicked);

            SetupPuzzleData(_puzzleData);
        }

        private void SetupPuzzleData(PuzzleData puzzleData)
        {
            puzzle_Txt.text = puzzleData.puzzleName;
            puzzle_Desc.text = puzzleData.description;
        }

        private void BrewButtonClicked()
        {
            var userPickedIng = new List<RawIngredient>();
            slots.ForEach(i => userPickedIng.Add(i.AssignedIngredient));
            IRecipe userRecipe = new Recipe(userPickedIng);
            IRecipe puzzleRecipe = new Recipe(_puzzleData.rawIngredients);
            var Result = validator.Validate(puzzleRecipe, userRecipe);
            Debug.Log($"Result : {Result.Correct}, Result.PartiallyCorrect : {Result.PartiallyCorrect}");
        }

        private void SpawnRawIngSlot()
        {
            var newIngSlot = Instantiate(ingSlot, slotParent);
            newIngSlot.Selected += HandleSlotSelected;
            newIngSlot.Deselected += HandleSlotDeselected;
            slots.Add(newIngSlot);
        }

        private void HandleSlotSelected(SelectableUi<RawIngredientSlot> _selectedSlot)
        {
            var slot = _selectedSlot as RawIngredientSlot;
            
            Debug.Log("Selected Slot : " + slot.gameObject.GetInstanceID());
            if (slot == null)
            {
                return;
            }

            if (IsSlotSelected() && selectedSlot != slot)
            {
                selectedSlot.Deselect();
            }

            selectedSlot = slot;
            Debug.Log("Selected Slot : " + _selectedSlot.gameObject.GetInstanceID());
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
            Debug.Log("Selected Slot : " + ing.ingredientName);
            selectedSlot.AssignIngredient(ing);
            selectedIng.Deselect();
            selectedSlot.Deselect();
            selectedIng = null;
            selectedSlot = null;
        }
    }
}