using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Interfaces;

// TODO : Fix the Recipe namespace
// TODO : Does it need to be a IRecipe ?
namespace AINPC.Scripts.Core.Gameplay.Recipe
{
    public class Recipe : IRecipe
    {
        public List<RawIngredient> rawIngredients { get; set; }
        public string recipeName { get; set; }
        public string description { get; set; }

        public Recipe(List<RawIngredient> rawIngredients)
        {
            this.rawIngredients = rawIngredients;
        }
    }
}