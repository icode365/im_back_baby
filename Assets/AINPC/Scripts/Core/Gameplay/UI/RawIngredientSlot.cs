using AINPC.Scripts.Core.Gameplay.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI
{
    public class RawIngredientSlot : MonoBehaviour
    {
        public Image slotImage = null;

        public bool IsSelected { get; private set; } = false;
        public RawIngredient AssignedIngredient { get; private set; } = null;

        public Button btn;

        private void Awake()
        {
            if (btn == null)
            {
                btn = GetComponent<Button>();
            }

            btn.onClick.RemoveListener(ToggleIsSelected);
            btn.onClick.AddListener(ToggleIsSelected);

            if (slotImage == null)
            {
                slotImage = GetComponent<Image>();
            }

            if (slotImage == null)
            {
                Debug.LogError("Slot Image reference is missing.");
            }
        }

        public void AssignIngredient(RawIngredient rawIng)
        {
            if (rawIng == null)
            {
                Debug.LogError("RawIngredient is null.");
                return;
            }

            AssignedIngredient = rawIng;

            if (slotImage == null)
            {
                Debug.LogWarning("Slot Image reference is null. Cannot display ingredient sprite.");
                return;
            }

            slotImage.sprite = rawIng.sprite;
            slotImage.enabled = true;
            
            ToggleIsSelected();
        }

        private void ToggleIsSelected()
        {
            IsSelected = !IsSelected;
        }

        private void OnDestroy()
        {
            if (btn != null)
            {
                btn.onClick.RemoveListener(ToggleIsSelected);
            }
        }
    }
}