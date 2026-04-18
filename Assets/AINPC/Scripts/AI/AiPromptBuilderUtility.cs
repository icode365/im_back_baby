using System.Text;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;

public static class AiPromptBuilder
{
    public static string BuildPrompt(
        string puzzleName,
        string puzzleDescription,
        IngredientsData ingredients)
    {
        var sb = new StringBuilder();

        sb.AppendLine("[PUZZLE] - Puzzle description explains which ingredients to mix but, ingredients hidden under a layer of metaphor.");
        sb.AppendLine($"Name: {puzzleName}");
        sb.AppendLine($"Description: {puzzleDescription}");
        sb.AppendLine();

        sb.AppendLine("[INGREDIENTS] - Available ingredients, each ingredient has properties which when mixed together, gives a reaction as a result.");
        foreach (var ingredientsData in ingredients.rawIngredients)
        {
            sb.AppendLine($"- {ingredientsData.ingredientName}, {ingredientsData.properties}");
        }
        sb.AppendLine();
        
        sb.AppendLine("You have to try to decrypt what the puzzle means and which Ingredients to use to get the desired results.");

        return sb.ToString();
    }
}
