namespace AINPC.Scripts.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GeminiAICreds", menuName = "Scriptable Objects/GeminiAICreds")]
    public class GeminiAISetting : ScriptableObject
    {
        public string apiKey = string.Empty;
    }
}