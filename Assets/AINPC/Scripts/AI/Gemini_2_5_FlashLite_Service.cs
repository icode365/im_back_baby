using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AINPC.Scripts.Core.AI.Interfaces;
using AINPC.Scripts.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace AINPC.Scripts.AI
{
    public class Gemini_2_5_FlashLite_Service : ILLMService
    {
        #region Gemini AI Class

        [Serializable]
        private class GeminiRequest
        {
            public SystemInstruction system_instruction;
            public List<Content> contents;
            // public GenerationConfig generationConfig;
        }

        [Serializable]
        public class GenerationConfig
        {
            public string[] responseModalities = { "AUDIO", "TEXT" };
            public SpeechConfig speechConfig;
        }

        [Serializable]
        public class SpeechConfig
        {
            public VoiceConfig voiceConfig;
        }

        [Serializable]
        public class VoiceConfig
        {
            public PrebuiltVoiceConfig prebuiltVoiceConfig;
        }

        [Serializable]
        public class PrebuiltVoiceConfig
        {
            public string voiceName = "Kore"; // Options: Puck, Charon, Kore, Fenrir, Aoede
        }

        [Serializable]
        private class SystemInstruction
        {
            public List<Part> parts;
        }

        [Serializable]
        private class Content
        {
            public List<Part> parts;
        }

        [Serializable]
        private class Part
        {
            public string text;
            public InlineData inlineData;
        }

        [Serializable]
        private class InlineData
        {
            public string mimeType;
            public string data;
        }

        [Serializable]
        private class GeminiResponse
        {
            public List<Candidate> candidates;
        }

        [Serializable]
        private class Candidate
        {
            public ResponseContent content;
        }

        [Serializable]
        private class ResponseContent
        {
            public List<Part> parts;
        }

        #endregion

        private AiSetting _aiSetting = null;
        public UnityEvent<string> OnResponseReceived = new();

        private const int GeminiSampleRate = 24000;
        private const int Channels = 1;

        public void Initialize(AiSetting aiSetting)
        {
            _aiSetting = aiSetting;
        }

        public async Task<ApiResponse> GetResponseAsync(string prompt, string systemInstruction)
        {
            ApiResponse apiResponse = new();
            Debug.Log("Prompt Received : " + prompt);


            var requestBody = BuildRequestBody(prompt, systemInstruction);

            string jsonBody = JsonUtility.ToJson(requestBody);
            // TODO : Use the GeminiAISetting.ModelCode for flexibility
            string url =
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent";

            using (var request = new UnityWebRequest(url, "POST"))
            {
                byte[] jsonRaw = Encoding.UTF8.GetBytes(jsonBody);

                request.uploadHandler = new UploadHandlerRaw(jsonRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-type", "application/json");

                string apiKey = _aiSetting.ApiKey;
                if (string.IsNullOrEmpty(apiKey))
                {
                    Debug.LogError(
                        "[GeminiService] API key is missing. Please set it in 'GeminiAICreds' asset or in 'gemini.key' file.");
                    apiResponse.error = "Missing API Key";
                    apiResponse.status = EAPIStatus.Error;
                    return apiResponse;
                }

                request.SetRequestHeader("X-goog-api-key", apiKey);

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    apiResponse.status = EAPIStatus.Processing;
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Request Returned : " + request.error + request.responseCode);
                    OnResponseReceived?.Invoke(request.error);

                    apiResponse.error = request.error;
                    apiResponse.status = EAPIStatus.Error;

                    return apiResponse;
                }

                try
                {
                    string responseBody = request.downloadHandler.text;
                    GeminiResponse geminiResponse = JsonUtility.FromJson<GeminiResponse>(responseBody);

                    string textResults = string.Empty;
                    AudioClip audioResults = null;

                    if (geminiResponse?.candidates != null && geminiResponse.candidates.Count > 0)
                    {
                        var parts = geminiResponse.candidates[0].content.parts;

                        foreach (var part in parts)
                        {
                            // Handle Text
                            if (!string.IsNullOrEmpty(part.text))
                            {
                                textResults += part.text;
                            }

                            // Handle Audio
                            if (part.inlineData != null && !string.IsNullOrEmpty(part.inlineData.data))
                            {
                                byte[] pcmBytes = Convert.FromBase64String(part.inlineData.data);
                                audioResults = CreateAudioClipFromPcm16(pcmBytes, GeminiSampleRate, Channels);
                            }
                        }

                        // if (parts != null && parts.Count > 0)
                        //     textResults = parts[0].text;
                    }

                    OnResponseReceived.Invoke(textResults);
                    apiResponse.response = textResults;
                    apiResponse.responseObject = audioResults;
                    apiResponse.status = EAPIStatus.Success;

                    return apiResponse;
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception Caught : " + e);
                    throw;
                }
            }
        }

        private GeminiRequest BuildRequestBody(string prompt, string systemInstruction)
        {
            return new GeminiRequest
            {
                system_instruction = new SystemInstruction()
                {
                    parts = new List<Part>
                    {
                        new Part
                        {
                            text = systemInstruction
                        }
                    }
                },
                contents = new List<Content>
                {
                    new Content
                    {
                        parts = new List<Part>
                        {
                            new Part
                            {
                                text = prompt
                            }
                        }
                    }
                },
                // generationConfig = new GenerationConfig
                // {
                //     responseModalities = new[] { "AUDIO", "TEXT" },
                //     speechConfig = new SpeechConfig
                //     {
                //         voiceConfig = new VoiceConfig
                //         {
                //             prebuiltVoiceConfig = new PrebuiltVoiceConfig { voiceName = "Kore" }
                //         }
                //     }
                // }
            };
        }

        private static AudioClip CreateAudioClipFromPcm16(byte[] pcmBytes, int sampleRate, int channels)
        {
            if (pcmBytes == null || pcmBytes.Length < 2) return null;
            int sampleCount = pcmBytes.Length / 2;
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                short sample16 = BitConverter.ToInt16(pcmBytes, i * 2);
                samples[i] = sample16 / 32768f;
            }

            AudioClip clip = AudioClip.Create("GeminiAudio", sampleCount / channels, channels, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}