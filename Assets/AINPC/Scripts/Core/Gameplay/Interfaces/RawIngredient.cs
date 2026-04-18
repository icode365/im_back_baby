using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AINPC.Scripts.Core.Gameplay.Interfaces
{
    // Does this need to be an interface?
    [Serializable]
    public class RawIngredient
    {
        public string ingredientName = "";
        public List<string> properties = new();
        public Sprite sprite = null;
    }
}