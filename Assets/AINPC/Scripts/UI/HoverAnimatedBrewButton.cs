using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class HoverFillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image fillImage;

    [SerializeField] private AudioClip hoverSfx;
    [SerializeField] private AudioSource audioSource;

    [Header("Tween Settings")] [SerializeField]
    private float hoverDuration = 0.35f;

    [SerializeField] private float exitDuration = 0.12f;

    private int _tweenId = -1;
    private bool _shouldAnimate = false;

    private void Awake()
    {
        fillImage.fillAmount = 0;
    }

    private void Start()
    {
        SetupAudioSource();
    }
    
    private void SetupAudioSource()
    {
        if (audioSource == null)
            audioSource = AddAudioSourceComponent();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.clip = hoverSfx;
    }

    private void PlayHoverSfx()
    {
        StopHoverSfx();
        audioSource.Play();
    }
    
    private void StopHoverSfx()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private AudioSource AddAudioSourceComponent() => gameObject.AddComponent<AudioSource>();
    
    public void SetShouldAnimate(bool shouldAnimate)
    {
        _shouldAnimate = shouldAnimate;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TryAnimate(1f);
        PlayHoverSfx();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TryAnimate(0f);
        StopHoverSfx();
    }

    private void TryAnimate(float target)
    {
        // todo : add this back in a way that, when we hover over the button, the animation should not play unless and untill the slots are filled
        // if (!_shouldAnimate) return;

        AnimateFill(target, hoverDuration, LeanTweenType.easeOutQuad);
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

        _tweenId = LeanTween.value(gameObject, startValue, target, duration)
            .setEase(ease)
            .setOnUpdate((float value) =>
            {
                if (fillImage != null)
                {
                    fillImage.fillAmount = value;
                }
            })
            .id;
    }
}