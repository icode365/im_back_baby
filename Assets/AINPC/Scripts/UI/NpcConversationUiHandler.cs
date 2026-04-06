using UnityEngine;
using UnityEngine.UI;

namespace AINPC.Scripts.UI
{
    public class NpcConversationUiHandler : MonoBehaviour
    {
        [SerializeField] private InputField inputField;

        public void ClearPromptInputField()
        {
            inputField.text = "";
        }
    }
}