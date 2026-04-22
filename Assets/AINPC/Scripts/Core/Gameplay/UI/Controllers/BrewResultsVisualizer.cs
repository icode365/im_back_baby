using AINPC.Scripts.Core.Gameplay.Validator;
using System;
using System.Collections;
using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Validator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI.Controllers
{
    public class BrewResultsVisualizer : MonoBehaviour
    {
        [Header("Result UI")] [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text resultLabel;
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private Button yayButton;

        [Header("Optional Ingredient Visuals")] [SerializeField]
        private Image resultImage;

        [SerializeField] private PropertyVisualRules propertyVisualRules;

        [Header("Tween")] [SerializeField] private RectTransform[] tweenTargets;
        [SerializeField] private float tweenDuration = 0.35f;

        [SerializeField] private Transform leftOffscreenTransform;
        [SerializeField] private Transform rightOffscreenTransform;
        
        
        [SerializeField] private Transform leftOnscreenTransform;
        [SerializeField] private Transform rightOnscreenTransform;
        // [SerializeField] private Vector2 offScreenOffset = new Vector2(0f, -1200f);

        private LTDescr _tweenRoutine;

        private void Start()
        {
            if (GlobalEventHandler.Instance != null)
            {
                GlobalEventHandler.Instance.RecipeValidated += Visualize;
            }

            if (tryAgainButton != null)
            {
                tryAgainButton.onClick.AddListener(OnTryAgainClicked);
            }

            if (yayButton != null)
            {
                yayButton.onClick.AddListener(OnYayClicked);
            }

            SetResultPanelVisible(false);
        }

        private void OnDestroy()
        {
            if (GlobalEventHandler.Instance != null)
            {
                GlobalEventHandler.Instance.RecipeValidated -= Visualize;
            }

            if (tryAgainButton != null)
            {
                tryAgainButton.onClick.RemoveListener(OnTryAgainClicked);
            }

            if (yayButton != null)
            {
                yayButton.onClick.RemoveListener(OnYayClicked);
            }
        }

        private void Visualize(ValidationResult results, RecipeProperties recipeProperties)
        {
            if (results == null)
            {
                SetResultPanelVisible(false);
                return;
            }

            SetResultPanelVisible(true);

            if (resultLabel != null)
            {
                resultLabel.text = results.Correct
                    ? "You're correct (even a broken clock is correct twice a day)"
                    : results.PartiallyCorrect
                        ? "Almost there"
                        : "Completely incorrect, you dummy!";
            }

            UpdateButtons(results);

            if (recipeProperties != null)
            {
                SetResultIngredientVisuals(recipeProperties.ResultantProperties);
            }
            
            TweenOnScreen();
        }

        private void UpdateButtons(ValidationResult results)
        {
            if (tryAgainButton != null)
            {
                tryAgainButton.gameObject.SetActive(!results.Correct);
            }

            if (yayButton != null)
            {
                yayButton.gameObject.SetActive(results.Correct);
            }
        }

        private void OnTryAgainClicked()
        {
            TweenOffScreen();
        }

        private void OnYayClicked()
        {
            TweenOffScreen();
        }

        private void TweenOffScreen()
        {
            if (_tweenRoutine != null)
            {
                LeanTween.cancel(_tweenRoutine.id);
            }

            _tweenRoutine = TweenOffScreenRoutine();
        }

        private void TweenOnScreen()
        {
            if (_tweenRoutine != null)
            {
                LeanTween.cancel(_tweenRoutine.id);
            }

            _tweenRoutine = TweenOnScreenRoutine();
        }
        
        private LTDescr TweenOffScreenRoutine()
        {
            var targets = tweenTargets ?? Array.Empty<RectTransform>();
            var startPositions = new Vector2[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    targets[i].anchoredPosition = leftOffscreenTransform.position;
                }
            }

            var tween = LeanTween.value(targets[0].gameObject, targets[0].anchoredPosition.x, leftOffscreenTransform.position.x,
                    tweenDuration)
                .setOnUpdate((float value)
                    =>
                {
                    targets[0].anchoredPosition = new(value, targets[0].anchoredPosition.y);
                })
                .setOnComplete(() => SetResultPanelVisible(false));
            return tween;
        }
        
        private LTDescr TweenOnScreenRoutine()
        {
            var targets = tweenTargets ?? Array.Empty<RectTransform>();

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    targets[i].anchoredPosition = leftOffscreenTransform.position;
                }
            }

            var tween = LeanTween.value(targets[0].gameObject, targets[0].anchoredPosition.x, leftOffscreenTransform.position.x,
                    tweenDuration)
                .setOnUpdate((float value)
                    =>
                {
                    targets[0].anchoredPosition = new(value, targets[0].anchoredPosition.y);
                });
            
            return tween;
        }

        private void SetResultPanelVisible(bool visible)
        {
            if (resultPanel)
            {
                resultPanel.SetActive(visible);
            }
        }

        public void SetResultIngredientVisuals(IEnumerable<string> propertyKeywords)
        {
            if (resultImage == null || propertyKeywords == null)
            {
                return;
            }

            Sprite chosenSprite = null;
            Color chosenTint = Color.white;

            foreach (var keyword in propertyKeywords)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    continue;
                }

                foreach (var rule in propertyVisualRules.PropertyVisualRulesList)
                {
                    if (rule == null || string.IsNullOrWhiteSpace(rule.Keyword))
                    {
                        continue;
                    }

                    if (keyword.IndexOf(rule.Keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        chosenSprite = rule.Sprite == null ? chosenSprite : rule.Sprite;
                        chosenTint = rule.IsColorOnly ? rule.Tint : chosenTint;

                        Debug.Log(keyword);
                    }
                }
            }

            if (chosenSprite != null)
            {
                resultImage.sprite = chosenSprite;
            }

            resultImage.color = chosenTint;
        }
    }
}