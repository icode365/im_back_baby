using System.Collections.Generic;
using AINPC.Scripts.Core.Gameplay.Interfaces;
using UnityEngine;

namespace AINPC.Scripts.Core.Gameplay.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RawIngredients", menuName = "Scriptable Objects/RawIngredients")]
    public class RawIngredients : ScriptableObject
    { 
        [SerializeField]
        public List<RawIngredient> rawIngredients = new();
    }
}