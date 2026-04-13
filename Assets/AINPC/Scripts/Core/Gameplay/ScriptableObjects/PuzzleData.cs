using System.Collections.Generic;
using UnityEngine;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using AINPC.Scripts.Core.Gameplay.ScriptableObjects;

namespace AINPC.Scripts.Core.Gameplay.Data
{
    [CreateAssetMenu(fileName = "PuzzleData", menuName = "Scriptable Objects/Puzzle Data")]
    public class PuzzleData : ScriptableObject
    {
        public string puzzleName;
        public RawIngredients Ingredients;

        public List<RawIngredient> rawIngredients = new();
        
        [TextArea(2, 5)]
        public string resultDescription;
    }
}