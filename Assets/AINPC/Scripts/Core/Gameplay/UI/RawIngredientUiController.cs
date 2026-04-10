using System;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI
{
    public class RawIngredientUiController : SelectableUi<RawIngredientUiController>
    {
        public RawIngredient RawIng { get; private set; } = null;
        
        public Image ingSprite = null;
        public TMP_Text ingName = null;

        public void Start()
        {
            if (btn == null)
            {
                Debug.LogError("Button reference is null. Check References");
                return;
            }

            btn.onClick.RemoveListener(ToggleSelection);
            btn.onClick.AddListener(ToggleSelection);
        }

        public RawIngredientUiController SetupRawIngUI(RawIngredient rawIng)
        {
            if (border == null || ingSprite == null || ingName == null || btn == null)
            {
                Debug.LogError("UI Ref is null. Check References");
                return null;
            }

            if (rawIng == null)
            {
                Debug.LogError("RawIngredient is null.");
                return null;
            }

            RawIng = rawIng;
            ingSprite.sprite = RawIng.sprite;
            ingName.text = RawIng.ingredientName;
            border.color = unselectedBorderColor;
            border.enabled = true;
            base.IsSelected = false;

            gameObject.name = "RI_" + RawIng.ingredientName;
            Debug.Log(rawIng.ingredientName + "," + RawIng.properties[0]);
            return this;
        }
    }
}