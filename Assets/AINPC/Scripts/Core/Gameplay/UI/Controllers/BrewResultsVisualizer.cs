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
        [Serializable]
        private class PropertyVisualRule
        {
            public string Keyword;
            public Sprite Sprite;
            public Color Tint = Color.white;
        }

        [Header("Result UI")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text resultLabel;
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private Button yayButton;

        [Header("Optional Ingredient Visuals")]
        [SerializeField] private Image resultImage;
        [SerializeField] private List<PropertyVisualRule> propertyVisualRules = new List<PropertyVisualRule>();

        [Header("Tween")]
        [SerializeField] private RectTransform[] tweenTargets;
        [SerializeField] private float tweenDuration = 0.35f;
        [SerializeField] private Vector2 offScreenOffset = new Vector2(0f, -1200f);

        private Coroutine _tweenRoutine;

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

        // Updated to receive the newly added payload
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
                    ? "Correct"
                    : results.PartiallyCorrect
                        ? "Partially correct"
                        : "Incorrect";
            }

            UpdateButtons(results);
            UpdateResultVisuals(results);

            if (recipeProperties != null)
            {
                SetResultIngredientVisuals(recipeProperties.ResultantProperties);
            }
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

        private void UpdateResultVisuals(ValidationResult results)
        {
            if (resultImage == null)
            {
                return;
            }

            if (results.Correct)
            {
                resultImage.color = Color.green;
            }
            else if (results.PartiallyCorrect)
            {
                resultImage.color = new Color(1f, 0.75f, 0.2f, 1f);
            }
            else
            {
                resultImage.color = Color.red;
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
                StopCoroutine(_tweenRoutine);
            }

            _tweenRoutine = StartCoroutine(TweenOffScreenRoutine());
        }

        private IEnumerator TweenOffScreenRoutine()
        {
            var targets = tweenTargets ?? Array.Empty<RectTransform>();
            var startPositions = new Vector2[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    startPositions[i] = targets[i].anchoredPosition;
                }
            }

            float elapsed = 0f;

            while (elapsed < tweenDuration)
            {
                elapsed += Time.deltaTime;
                float t = tweenDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / tweenDuration);
                t = Mathf.SmoothStep(0f, 1f, t);

                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] == null)
                    {
                        continue;
                    }

                    targets[i].anchoredPosition = Vector2.Lerp(startPositions[i], startPositions[i] + offScreenOffset, t);
                }

                yield return null;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    targets[i].anchoredPosition = startPositions[i] + offScreenOffset;
                }
            }

            SetResultPanelVisible(false);
            _tweenRoutine = null;
        }

        private void SetResultPanelVisible(bool visible)
        {
            if (resultPanel != null)
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

                foreach (var rule in propertyVisualRules)
                {
                    if (rule == null || string.IsNullOrWhiteSpace(rule.Keyword))
                    {
                        continue;
                    }

                    if (keyword.IndexOf(rule.Keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        chosenSprite = rule.Sprite ?? chosenSprite;
                        chosenTint = MultiplyColors(chosenTint, rule.Tint);
                    }
                }
            }

            if (chosenSprite != null)
            {
                resultImage.sprite = chosenSprite;
            }

            resultImage.color = chosenTint;
        }

        private static Color MultiplyColors(Color a, Color b)
        {
            return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }
    }
}
