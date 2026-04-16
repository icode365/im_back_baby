using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI
{
    public class IngredientSlot : SelectableUi<IngredientSlot>
    {
        public Image slotImage = null;
        public Interfaces.RawIngredient AssignedIngredient { get; private set; } = null;

        private void Awake()
        {
            base.Awake();
            
            if (slotImage == null)
            {
                slotImage = GetComponent<Image>();
            }

            if (slotImage == null)
            {
                Debug.LogError("Slot Image reference is missing.");
            }
        }

        public void AssignIngredient(Interfaces.RawIngredient rawIng)
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