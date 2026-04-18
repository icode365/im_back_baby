using System;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.Gameplay.UI.Base
{
    public class SelectableUi<T> : MonoBehaviour
    {
        public event Action<SelectableUi<T>> Selected;
        public event Action<SelectableUi<T>> Deselected;
        public bool IsSelected { get; protected set; } = false;
        
        public Image border = null;
        public Button btn = null;
        
        public Color selectedBorderColor = Color.green;
        public Color unselectedBorderColor = Color.white;

        protected virtual void Awake()
        {
            if (btn == null)
            {
                btn = GetComponent<Button>();
            }

            if (btn == null)
            {
                Debug.LogError($"{name}: Missing Button reference.");
                return;
            }

            btn.onClick.AddListener(ToggleSelection);
        }


        protected void ToggleSelection()
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

        protected virtual void Select()
        {
            IsSelected = true;

            if (border == null) return;
            
            border.enabled = true;
            border.color = selectedBorderColor;
        }

        public virtual void Deselect()
        {
            IsSelected = false;

            if (border == null) return;
            
            border.enabled = true;
            border.color = unselectedBorderColor;
        }
    }
}