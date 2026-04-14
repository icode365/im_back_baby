using System.Collections.Generic;

namespace AINPC.Scripts.Core.Gameplay.Interfaces
{
    public interface IRecipe
    {
        public List<RawIngredient> rawIngredients { get; set; }
        public string recipeName { get; set; }
        public string description { get; set; }
    }
}