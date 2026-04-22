using AINPC.Scripts.Core.Gameplay.Validator;
using System;
using System.Collections.Generic;
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

        private Image leftImg;
        private Image rightImg;

        [SerializeField] private PropertyVisualRules propertyVisualRules;

        [Header("Tween")] [SerializeField] private RectTransform leftImage;
        [SerializeField] private RectTransform rightImage;
        [SerializeField] private float tweenDuration = 0.35f;

        [SerializeField] private Transform leftOffscreenTransform;
        [SerializeField] private Transform rightOffscreenTransform;

        [SerializeField] private Transform leftOnscreenTransform;
        [SerializeField] private Transform rightOnscreenTransform;

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

            if (leftImage != null && rightImage != null)
            {
                leftImg = leftImage.GetComponent<Image>();
                rightImg = rightImage.GetComponent<Image>();
            }
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

            TweenIn();
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
            Dismiss();
        }

        private void OnYayClicked()
        {
            Dismiss();
        }

        private void Dismiss()
        {
            TweenOut();
            // todo : add wait for tweening to complete
            SetResultPanelVisible(false);
        }

        private void TweenIn()
        {
            TweenRect(leftImage, leftOffscreenTransform.localPosition, leftOnscreenTransform.localPosition);
            TweenRect(rightImage, rightOffscreenTransform.localPosition, rightOnscreenTransform.localPosition);
        }

        private void TweenOut()
        {
            TweenRect(leftImage, leftOnscreenTransform.localPosition, leftOffscreenTransform.localPosition);
            TweenRect(rightImage, rightOnscreenTransform.localPosition, rightOffscreenTransform.localPosition);
        }

        private void TweenRect(RectTransform target, Vector2 from, Vector2 to)
        {
            if (target == null)
            {
                return;
            }

            target.anchoredPosition = from;
            LeanTween.cancel(target.gameObject);
            LeanTween.move(target, to, tweenDuration).setEaseOutCubic();
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
            if (leftImg == null || rightImg == null || propertyKeywords == null)
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
                leftImg.sprite = chosenSprite;
                leftImg.SetNativeSize();
                
                rightImg.sprite = chosenSprite;
                rightImg.SetNativeSize();
            }

            leftImg.color = chosenTint;
            rightImg.color = chosenTint;
        }
    }
}