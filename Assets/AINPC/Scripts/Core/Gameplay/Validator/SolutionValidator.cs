using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay.Validator
{
    public class ValidationResult
    {
        public bool Correct { get; private set; } = false;
        public bool PartiallyCorrect { get; private set; } = false;

        public void SetResult(bool correct, bool partiallyCorrect)
        {
            Correct = correct;
            PartiallyCorrect = partiallyCorrect;
        }
    }

    public class RecipeProperties
    {
        public List<string> ResultantProperties  { get; private set; } = new();

        public void SetProperties(List<string> properties)
        {
            ResultantProperties = properties;
        }
    }
    
    public class SolutionValidator : IValidator
    {
        public ValidationResult Validate(IRecipe solution, IRecipe recipe)
        {
            ValidationResult result = new ValidationResult();

            if (solution == null || recipe == null)
            {
                return result;
            }

            var solutionIngredients = solution.rawIngredients;
            var recipeIngredients = recipe.rawIngredients;

            if (solutionIngredients == null || recipeIngredients == null)
            {
                return result;
            }

            // Check for exact match
            bool isCorrect = IsExactMatch(solutionIngredients, recipeIngredients);

            if (isCorrect)
            {
                result.SetResult(true, false);
                
                return result;
            }

            // Check for partial match (at least one ingredient matches)
            bool isPartiallyCorrect = IsPartiallyCorrect(solutionIngredients, recipeIngredients);
            
            
            result.SetResult(false, isPartiallyCorrect);
            
            return result;
        }

        private bool IsExactMatch(List<RawIngredient> solution, List<RawIngredient> recipe)
        {
            if (solution.Count != recipe.Count) return false;

            var solutionSorted = new List<RawIngredient>(solution);
            var recipeSorted = new List<RawIngredient>(recipe);

            solutionSorted.Sort((a, b) => string.Compare(a.ingredientName, b.ingredientName, System.StringComparison.Ordinal));
            recipeSorted.Sort((a, b) => string.Compare(a.ingredientName, b.ingredientName, System.StringComparison.Ordinal));

            for (int i = 0; i < solutionSorted.Count; i++)
            {
                if (!AreIngredientsEqual(solutionSorted[i], recipeSorted[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsPartiallyCorrect(List<RawIngredient> solution, List<RawIngredient> recipe)
        {
            foreach (var solIng in solution)
            {
                foreach (var recIng in recipe)
                {
                    if (AreIngredientsEqual(solIng, recIng)) return true;
                }
            }
            return false;
        }

        private bool AreIngredientsEqual(RawIngredient a, RawIngredient b)
        {
            if (a == null || b == null) return a == b;
            return a.ingredientName == b.ingredientName;
        }
    }
}