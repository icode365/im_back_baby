using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay;
using AINPC.Scripts.Core.Gameplay.UI;
using AINPC.Scripts.Core.Gameplay.UI.Handlers;
using AINPC.Scripts.Core.Gameplay.Validator;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AINPC.Scripts.UI.Tutorial
{
    public class TutorialHandler : MonoBehaviour
    {
        [SerializeField] private List<string> tutorials = new();

        [SerializeField] private TMP_Text tutorialsText;
        [SerializeField] private CanvasGroup tutorialCanvasGroup;

        [Header("Tutorial Dependencies")]
        [SerializeField] private PuzzlePanelEventHandler puzzlePanelEventHandler;
        [SerializeField] private InputActionReference mouseClickAction;

        private int currentStepIndex = -1;

        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (tutorialCanvasGroup != null)
                tutorialCanvasGroup.alpha = 0;
            
            MoveToNextStep();
        }

        private void MoveToNextStep()
        {
            RemoveListenerForLastStep();
            currentStepIndex++;

            if (currentStepIndex < tutorials.Count)
            {
                SetStepText(tutorials[currentStepIndex]);
                AnimateText(tutorialsText.gameObject);
                AddListenerForNextStep();
            }
            else
            {
                // Tutorial finished
                if (tutorialCanvasGroup != null)
                {
                    LeanTween.alphaCanvas(tutorialCanvasGroup, 0f, 0.5f)
                        .setEaseInOutCubic()
                        .setOnComplete(() => gameObject.SetActive(false));
                }
            }
        }

        private void RemoveListenerForLastStep()
        {
            if (currentStepIndex < 0) return;

            // Remove mouse click listener anyway as it might have been added by default
            if (mouseClickAction != null)
                mouseClickAction.action.performed -= OnMouseClickPerformed;
            
            if (currentStepIndex == 3) // Ingredient placement step
            {
                if (puzzlePanelEventHandler != null)
                    puzzlePanelEventHandler.IngredientSelected -= OnIngredientSelected;
            }
            else if (currentStepIndex == 5) // Brew step
            {
                if (GlobalEventHandler.Instance != null)
                    GlobalEventHandler.Instance.RecipeValidated -= OnRecipeValidated;
            }
        }

        private void AddListenerForNextStep()
        {
            if (currentStepIndex == 3) // Ingredient placement step
            {
                if (puzzlePanelEventHandler != null)
                    puzzlePanelEventHandler.IngredientSelected += OnIngredientSelected;
            }
            else if (currentStepIndex == 5) // Brew step
            {
                if (GlobalEventHandler.Instance != null)
                    GlobalEventHandler.Instance.RecipeValidated += OnRecipeValidated;
            }
            else
            {
                // Default is mouse click to continue
                if (mouseClickAction != null)
                    mouseClickAction.action.performed += OnMouseClickPerformed;
            }
        }

        private void OnMouseClickPerformed(InputAction.CallbackContext context)
        {
            MoveToNextStep();
        }

        private void OnIngredientSelected(SelectableRawIngredient ingredient)
        {
            MoveToNextStep();
        }

        private void OnRecipeValidated(ValidationResult result, RecipeProperties properties)
        {
            MoveToNextStep();
        }

        private void SetStepText(string tutorialStatement)
        {
            if (!string.IsNullOrEmpty(tutorialStatement))
                tutorialsText.text = tutorialStatement;
        }

        private void AnimateText(GameObject target)
        {
            if (tutorialCanvasGroup == null) return;

            LeanTween.cancel(tutorialCanvasGroup.gameObject);
            tutorialCanvasGroup.alpha = 0;
            
            LeanTween.alphaCanvas(tutorialCanvasGroup, 1f, 0.5f)
                .setEaseInOutCubic();
            
            // Subtle pop animation for the text itself
            target.transform.localScale = Vector3.one * 0.95f;
            LeanTween.scale(target, Vector3.one, 0.5f)
                .setEaseOutBack();
        }

        private void OnDestroy()
        {
            if (mouseClickAction != null)
                mouseClickAction.action.performed -= OnMouseClickPerformed;
            
            if (puzzlePanelEventHandler != null)
                puzzlePanelEventHandler.IngredientSelected -= OnIngredientSelected;

            if (GlobalEventHandler.Instance != null)
                GlobalEventHandler.Instance.RecipeValidated -= OnRecipeValidated;
        }
    }
}