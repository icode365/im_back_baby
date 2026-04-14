using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Interfaces;

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
