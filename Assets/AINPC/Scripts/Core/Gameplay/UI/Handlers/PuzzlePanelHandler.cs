using System;
using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Data;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI.Handlers
{
    public class PuzzlePanelHandler : MonoBehaviour
    {
        public Button brewButton = null;
        public TMP_Text puzzleNameText = null;
        public TMP_Text puzzleDescriptionTxt = null;

        public Transform ingHorizontalContainer = null;
        public SelectableRawIngredient ingredientPrefab = null;

        public Transform slotsHorizontalContainer = null;
        public IngredientSlot slotPrefab = null;
        
        private List<IngredientSlot> _slots = new();

        public event Action<IngredientSlot> SlotSelected;
        public event Action<IngredientSlot> SlotDeselected;
        public event Action<SelectableRawIngredient> IngredientSelected;
        public event Action<SelectableRawIngredient> IngredientDeselected;

        public event Action BrewButtonClicked;

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
            brewButton.onClick.AddListener(() => BrewButtonClicked?.Invoke());
        }

        private void RemoveListeners()
        {
            brewButton.onClick.RemoveAllListeners();
        }

        public void Initialize(PuzzleData puzzleData, IngredientsData ingredientsData)
        {
            SetupPuzzleData(puzzleData);
            
            ingredientsData.rawIngredients.ForEach(SpawnIngredients);
            puzzleData.rawIngredients.ForEach(_ => SpawnIngredientSlot());
        }

        public void SetupPuzzleData(PuzzleData puzzleData)
        {
            puzzleNameText.text = puzzleData.puzzleName;
            puzzleDescriptionTxt.text = puzzleData.description;
        }

        public List<Interfaces.RawIngredient> GetRawIngUserInputList()
        {
            var userPickedIng = new List<Interfaces.RawIngredient>();
            _slots.ForEach(i => userPickedIng.Add(i.AssignedIngredient));
            return userPickedIng;
        }

        private void SpawnIngredientSlot()
        {
            var newSlot = Instantiate(slotPrefab, slotsHorizontalContainer);
            newSlot.Selected += (_) => SlotSelected?.Invoke(newSlot);
            newSlot.Deselected += (_) => SlotDeselected?.Invoke(newSlot);
            _slots.Add(newSlot);
        }
        
        private void SpawnIngredients(Interfaces.RawIngredient ing)
        {
            var ingUi = Instantiate(ingredientPrefab, ingHorizontalContainer);
            ingUi.SetupRawIngUI(ing);
            ingUi.Selected += (_) => IngredientSelected?.Invoke(ingUi);
            ingUi.Deselected += (_) => IngredientDeselected?.Invoke(ingUi);
        }
    }
}