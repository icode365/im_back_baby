using System;
using System.Collections.Generic;
using System.Linq;
using AINPC.Scripts.Core.Gameplay.Data;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI.Handlers
{
    public class PuzzlePanelEventHandler : MonoBehaviour
    {
        private LTDescr _currentTweens = null;
        
        public Button brewButton = null;
        public TMP_Text puzzleNameText = null;
        public TMP_Text puzzleDescriptionTxt = null;

        public Transform ingHorizontalContainer = null;
        public SelectableRawIngredient ingredientPrefab = null;

        public Transform slotsHorizontalContainer = null;
        public IngredientSlot slotPrefab = null;

        [SerializeField]private Transform snackbar;
        private List<IngredientSlot> _slots = new();

        public event Action<IngredientSlot> SlotSelected;
        public event Action<IngredientSlot> SlotDeselected;
        public event Action<SelectableRawIngredient> IngredientSelected;
        public event Action<SelectableRawIngredient> IngredientDeselected;

        public event Action BrewButtonClicked;

        private void Start()
        {
            AddListeners();
            HideSnackbar();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
        
        private void AddListeners()
        {
            brewButton.onClick.AddListener(OnBrewButtonClicked);
        }

        private void RemoveListeners()
        {
            brewButton.onClick.RemoveAllListeners();
        }

        public void Initialize(PuzzleData puzzleData, IngredientsData ingredientsData)
        {
            ingredientsData.rawIngredients.ForEach(SpawnIngredients);
            puzzleData.rawIngredients.ForEach(_ => SpawnIngredientSlot());
         
            SetupPuzzleData(puzzleData);
        }

        public void ResetPuzzle()
        {
            foreach (var ingredientSlot in _slots)
            {
                ingredientSlot.Reset();
            }
        }
        
        public void SetupPuzzleData(PuzzleData puzzleData)
        {
            puzzleNameText.text = puzzleData.puzzleName;
            puzzleDescriptionTxt.text = puzzleData.description;
        }

        public List<Interfaces.RawIngredient> GetRawIngUserInputList()
        {
            var userPickedIng = new List<RawIngredient>();
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
        
        private void SpawnIngredients(RawIngredient ing)
        {
            var ingUi = Instantiate(ingredientPrefab, ingHorizontalContainer);
            ingUi.SetupRawIngUI(ing);
            ingUi.Selected += (_) => IngredientSelected?.Invoke(ingUi);
            ingUi.Deselected += (_) => IngredientDeselected?.Invoke(ingUi);
        }

        private void OnBrewButtonClicked()
        {
            if (CheckRightConditions())
            {
                BrewButtonClicked?.Invoke();
            }
            else
            {
                CancelTweens();
                _currentTweens = ShowSnackbarWithAutoHide();
            }
        }

        private void CancelTweens()
        {
            if (_currentTweens != null)
                LeanTween.cancel(_currentTweens.id);
        }
        
        private LTDescr ShowSnackbarWithAutoHide()
        {
            return LeanTween.scale(snackbar.gameObject, Vector3.one, 0.5f)
                .setEaseInOutCubic()
                .setOnComplete( () => LeanTween.delayedCall(1f, HideSnackbar));
        }

        private void HideSnackbar()
        {
            LeanTween.scale(snackbar.gameObject, Vector3.zero, 0.25f);
        }

        private bool CheckRightConditions()
        {
            return _slots.All(slot => slot.Assigned);
        }
    }
}