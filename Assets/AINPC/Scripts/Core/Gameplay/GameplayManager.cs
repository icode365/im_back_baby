using System;
using System.Linq;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        // Start from Introduction panel
            // Show welcome screen
            // Introduction of Goal
            // Intro of Conflict
            // Intro of NPCs their limitations and constraints
        // Start Gameplay
            // Start the puzzle
            // Show the NPC options
            // User selects which NPC abilities to use
            // User enters prompts to get the correct answers
            // User enters the answer in the puzzle to open solve the puzzle

            public RawIngredients ingredients = null;

            private void Start()
            {
                ingredients.rawIngredients.ForEach(ing => Debug.Log(ing.ingredientName + "," + ing.properties[0] + "\n"));
            }
    }
}