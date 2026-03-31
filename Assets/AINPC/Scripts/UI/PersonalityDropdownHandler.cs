using System.Collections.Generic;
using System.Linq;
using AINPC.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.Core.UI
{
    public class PersonalityDropdownHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown personaDropdown;
        public event System.Action<string> onPersonaChanged;

        private void Start()
        {
            personaDropdown.onValueChanged.AddListener(OnPersonaChanged);
        }

        public void Populate(List<string> personas)
        {
            personaDropdown.AddOptions(personas);
        }

        private void OnPersonaChanged(int i)
        {
            var option = personaDropdown.options[i].text;
            onPersonaChanged?.Invoke(option);
        }
    }
}