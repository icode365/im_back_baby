using System.Collections.Generic;

namespace AINPC.Scripts.Core.Gameplay.Interfaces
{
    public interface IRecipe
    {
        public List<RawIngredient> rawIngredients { get; set; }
        public string recipeName { get; set; } // Question : Why can't this be private set?
        public string description { get; set; }
    }
}