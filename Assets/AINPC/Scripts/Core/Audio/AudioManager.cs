using System;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip genericButtonClickSfx;
        [SerializeField] private AudioSource buttonClickSource;

        void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            RemoveAllButtonListners();
        }

        public void Initialize()
        {
            AssignGenericButtonClickSfx();
        }

        private void AssignGenericButtonClickSfx()
        {
            var buttons = FindObjectsOfTypeAll(typeof(Button));
            Debug.Log("Buttons Count :" + buttons.Length);
            foreach (var item in buttons)
            {
                var button = item as Button;
                button.onClick.AddListener(PlayGenericButtonClickSfx);
            }
        }

        private void RemoveAllButtonListners()
        {
            var buttons = FindObjectsOfTypeAll(typeof(Button));
            foreach (var item in buttons)
            {
                var button = item as Button;
                button.onClick.RemoveListener(PlayGenericButtonClickSfx);
            }
        }

        private void PlayGenericButtonClickSfx()
        {
            buttonClickSource.clip = genericButtonClickSfx;
            
            if (buttonClickSource.isPlaying)
                buttonClickSource.Stop();
            buttonClickSource.Play();
        }
    }
}