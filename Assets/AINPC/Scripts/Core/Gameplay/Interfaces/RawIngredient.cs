using System;
using System.Collections.Generic;

namespace AINPC.Scripts.Core.Gameplay.Interfaces
{
    [Serializable]
    public class RawIngredient
    {
        public string ingredientName = "";
        public List<string> properties = new();
    }
}