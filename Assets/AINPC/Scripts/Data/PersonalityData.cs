using UnityEngine;

namespace AINPC.Scripts.Data
{
    [CreateAssetMenu(fileName = "PersonalityData", menuName = "Scriptable Objects/Personality_Data")]
    public class PersonalityData : ScriptableObject
    {
        public string npcName;
        public string backstory;

        public string[] personalityTraits;
        public string[] speechPatterns;
        public string[] behavioralRules;
    }
}