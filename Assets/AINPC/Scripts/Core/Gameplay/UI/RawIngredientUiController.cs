using System;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI
{
    public class RawIngredientUiController : MonoBehaviour
    {
        public RawIngredient RawIng { get; private set; } = null;
        
        public Image border = null;
        public Image ingSprite = null;
        public TMP_Text ingName = null;
        public Button btn = null;

        public Color selectedBorderColor = Color.green;
        public Color unselectedBorderColor = Color.white;

        public event Action<RawIngredientUiController> Selected;
        public event Action<RawIngredientUiController> Deselected;
        public bool IsSelected { get; private set; } = false;

        public void Start()
        {
            if (btn == null)
            {
                Debug.LogError("Button reference is null. Check References");
                return;
            }

            btn.onClick.RemoveListener(ToggleIsSelected);
            btn.onClick.AddListener(ToggleIsSelected);
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
            IsSelected = false;

            gameObject.name = "RI_" + RawIng.ingredientName;
            Debug.Log(rawIng.ingredientName + "," + RawIng.properties[0]);
            return this;
        }

        private void ToggleIsSelected()
        {
            if (IsSelected)
            {
                Deselect();
                Deselected?.Invoke(this);
            }
            else
            {
                Select();
                Selected?.Invoke(this);
            }
        }

        public void Select()
        {
            IsSelected = true;

            if (border != null)
            {
                border.enabled = true;
                border.color = selectedBorderColor;
            }
        }

        public void Deselect()
        {
            IsSelected = false;

            if (border != null)
            {
                border.enabled = true;
                border.color = unselectedBorderColor;
            }
        }
    }
}