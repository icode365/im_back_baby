using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AINPC.Scripts.UI
{
    public class HoverFillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image fillImage;

        [Header("Tween Settings")] [SerializeField]
        private float hoverDuration = 0.35f;

        [SerializeField] private float exitDuration = 0.12f;

        private int _tweenId = -1;
        private float _targetFill;

        private void Awake()
        {
            if (fillImage != null)
            {
                _targetFill = fillImage.fillAmount;
            }

            fillImage.fillAmount = 0;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AnimateFill(1f, hoverDuration, LeanTweenType.easeOutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AnimateFill(0f, exitDuration, LeanTweenType.easeOutQuad);
        }

        private void AnimateFill(float target, float duration, LeanTweenType ease)
        {
            if (fillImage == null)
            {
                return;
            }

            if (_tweenId != -1)
            {
                LeanTween.cancel(_tweenId);
            }

            float startValue = fillImage.fillAmount;
            _targetFill = target;

            _tweenId = LeanTween.value(gameObject, startValue, target, duration)
                .setEase(ease)
                .setOnUpdate((float value) =>
                {
                    if (fillImage)
                    {
                        fillImage.fillAmount = value;
                    }
                })
                .id;
        }
    }
}