namespace AINPC.Scripts.Data
{
    using UnityEngine;
    using System.IO;

    [CreateAssetMenu(fileName = "GeminiAICreds", menuName = "Scriptable Objects/GeminiAICreds")]
    public class AiSetting : ScriptableObject
    {
        [Header("API Configuration")]
        [Tooltip(
            "The API key for Gemini. If left empty, the system will try to load it from 'gemini.key' in the project root.")]
        [SerializeField]
        private string apiKey = string.Empty;

        public string modelCode = "gemini-2.0-flash-lite";

        /// <summary>
        /// Gets the Gemini API key. If the field is empty in the inspector, 
        /// it attempts to load it from a local file 'gemini.key' (which is git-ignored).
        /// </summary>
        public string ApiKey
        {
            get
            {
                // if (!string.IsNullOrEmpty(apiKey))
                //     return apiKey;
                // Debug.LogWarning(
                //     "[AiSetting] apiKey is null or empty.");

                return LoadKeyFromFile();
            }
        }

        private string LoadKeyFromFile()
        {
            // Path to gemini.key in the root directory (outside Assets)
            string rootPath = Path.Combine(Application.dataPath, "..", "gemini.key");

            Debug.Log(
                "[AiSetting] Searching for gemini.key at path " + rootPath);
            if (File.Exists(rootPath))
            {
                try
                {
                    string content = File.ReadAllText(rootPath).Trim();
                    if (!string.IsNullOrEmpty(content))
                    {
                        Debug.Log(
                            "[AiSetting] found gemini.key content : " + content);
                        return content;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[AiSetting] Failed to load Gemini API key from file: {e.Message}");
                }
            }

            Debug.LogWarning(
                "[AiSetting] Gemini API key not found in 'gemini.key' file. Please set it in the inspector or in the file.");

            return string.Empty;
        }
    }
}