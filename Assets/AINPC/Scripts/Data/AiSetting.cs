namespace AINPC.Scripts.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GeminiAICreds", menuName = "Scriptable Objects/GeminiAICreds")]
    public class AiSetting : ScriptableObject
    {
        public string apiKey = string.Empty;
        public string modelCode = string.Empty;
    }
}