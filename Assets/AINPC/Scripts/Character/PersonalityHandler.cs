using System.Collections.Generic;
using System.Linq;
using System.Text;
using AINPC.Scripts.Data;
using UnityEngine;

namespace AINPC.Scripts.Character
{
    public class PersonalityHandler : MonoBehaviour
    {
        private PersonalityData currentPersonaData = null;

        [SerializeField] private List<PersonalityData> availablePersonas = new();

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (availablePersonas.Count > 0) 
                currentPersonaData = availablePersonas[0];
        }

        public PersonalityData GetCurrentPersona() => currentPersonaData;

        public List<PersonalityData> GetAvailablePersonas() => availablePersonas;

        public List<string> GetAvailablePersonaNames() => availablePersonas.Select(p => p.npcName).ToList();

        public PersonalityData GetPersonaDataFor(string name)
        {
            PersonalityData p = null;

            p = availablePersonas.Find(persona => persona.npcName == name);
            
            return p;
        }

        public void SetCurrentPersona(string name)
        {
            currentPersonaData = GetPersonaDataFor(name);
        }
        
        public string BuildPersonaPrompt()
        {
            if (currentPersonaData == null)
            {
                return "Respond naturally and helpfully in very few words.";
            }

            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(currentPersonaData.npcName))
            {
                sb.AppendLine($"You are {currentPersonaData.npcName}.");
            }

            if (!string.IsNullOrWhiteSpace(currentPersonaData.backstory))
            {
                sb.AppendLine();
                sb.AppendLine("Backstory:");
                sb.AppendLine(currentPersonaData.backstory.Trim());
            }

            if (currentPersonaData.personalityTraits != null && currentPersonaData.personalityTraits.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Personality traits:");
                foreach (var trait in currentPersonaData.personalityTraits)
                {
                    if (!string.IsNullOrWhiteSpace(trait))
                        sb.AppendLine($"- {trait.Trim()}");
                }
            }

            if (currentPersonaData.speechPatterns != null && currentPersonaData.speechPatterns.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Speech patterns:");
                foreach (var pattern in currentPersonaData.speechPatterns)
                {
                    if (!string.IsNullOrWhiteSpace(pattern))
                        sb.AppendLine($"- {pattern.Trim()}");
                }
            }

            if (currentPersonaData.behavioralRules != null && currentPersonaData.behavioralRules.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Behavioral rules:");
                foreach (var rule in currentPersonaData.behavioralRules)
                {
                    if (!string.IsNullOrWhiteSpace(rule))
                        sb.AppendLine($"- {rule.Trim()}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("Stay in character and respond according to the personality above.");

            return sb.ToString().Trim();
        }
    }
}