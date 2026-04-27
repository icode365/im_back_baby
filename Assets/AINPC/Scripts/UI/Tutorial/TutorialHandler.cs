using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay;
using AINPC.Scripts.Core.Gameplay.UI;
using AINPC.Scripts.Core.Gameplay.UI.Handlers;
using AINPC.Scripts.Core.Gameplay.Validator;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AINPC.Scripts.Core.UI.Tutorial
{
    public class TutorialHandler : MonoBehaviour
    {
        [SerializeField] private List<string> tutorials = new()
        {
            "You are an Alchemist.\n\nNot a follower of instructions.\nA seeker of truth beneath symbols.",
            "You observe.\nYou test.\nYou learn.",
            "Each ingredient holds a nature.\nHeat. Decay. Motion. Binding.",
            "Place two or three ingredients into the vessel.\n\nOrder does not matter.",
            "The recipe will speak in riddles, so that these doesn't land in some amataur's hands.",
            "When your mind is steady—\n\nPress “Brew.” to start mixing",
            "The vessel will answer.",
            "Reveal the hidden reaction.",
            "Proceed wisely."
        };

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