using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AINPC.Scripts.Core.AI.Interfaces;
using AINPC.Scripts.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace AINPC.Scripts.AI
{
    public class Gemini_2_5_FlashLite_Tts_Service : ITtsService
    {
        [Serializable]
        private class GeminiTtsRequest
        {
            public List<Content> contents;
            public GenerationConfig generationConfig;
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
        }

        [Serializable]
        private class GenerationConfig
        {
            public string[] responseModalities;
            public SpeechConfig speechConfig;
        }

        [Serializable]
        private class SpeechConfig
        {
            public VoiceConfig voiceConfig;
        }

        [Serializable]
        private class VoiceConfig
        {
            public PrebuiltVoiceConfig prebuiltVoiceConfig;
        }

        [Serializable]
        private class PrebuiltVoiceConfig
        {
            public string voiceName;
        }

        [Serializable]
        private class GeminiTtsResponse
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
            public List<ResponsePart> parts;
        }

        [Serializable]
        private class ResponsePart
        {
            public InlineData inlineData;
        }

        [Serializable]
        private class InlineData
        {
            public string mimeType;
            public string data;
        }

        private const int GeminiSampleRate = 24000;
        private const int Channels = 1;

        private AiSetting aiSetting = null;
        private event System.Action OnAudioRecieved;

        public void Initialize(AiSetting _aiSetting)
        {
            aiSetting = _aiSetting;
        }
    
        public async Task<ApiResponse> RequestAudioFor(string text)
        {
            ApiResponse apiResponse = new();
            apiResponse.responseType = ResponseType.AUDIO;

            if (aiSetting == null)
            {
                apiResponse.error = "AI settings are not initialized.";
                apiResponse.status = EAPIStatus.Error;
                return apiResponse;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                apiResponse.error = "Input text is NULL or empty.";
                apiResponse.status = EAPIStatus.Error;
                return apiResponse;
            }

            string modelCode = string.IsNullOrWhiteSpace(aiSetting.modelCode)
                ? "gemini-2.5-flash-preview-tts"
                : aiSetting.modelCode;

            var requestBody = new GeminiTtsRequest
            {
                contents = new List<Content>
                {
                    new Content
                    {
                        parts = new List<Part>
                        {
                            new Part { text = text }
                        }
                    }
                },
                generationConfig = new GenerationConfig
                {
                    responseModalities = new[] { "AUDIO" },
                    speechConfig = new SpeechConfig
                    {
                        voiceConfig = new VoiceConfig
                        {
                            prebuiltVoiceConfig = new PrebuiltVoiceConfig
                            {
                                voiceName = "Kore"
                            }
                        }
                    }
                }
            };

            string jsonBody = JsonUtility.ToJson(requestBody);
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelCode}:generateContent";

            using (var request = new UnityWebRequest(url, "POST"))
            {
                byte[] jsonRaw = Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(jsonRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-type", "application/json");
                request.SetRequestHeader("X-goog-api-key", aiSetting.apiKey);

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    apiResponse.status = EAPIStatus.Processing;
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    apiResponse.error = request.error;
                    apiResponse.status = EAPIStatus.Error;
                    return apiResponse;
                }

                try
                {
                    string responseBody = request.downloadHandler.text;
                    GeminiTtsResponse geminiResponse = JsonUtility.FromJson<GeminiTtsResponse>(responseBody);

                    string audioDataBase64 = string.Empty;
                    string mimeType = string.Empty;

                    if (geminiResponse?.candidates != null && geminiResponse.candidates.Count > 0)
                    {
                        var parts = geminiResponse.candidates[0].content?.parts;
                        if (parts != null && parts.Count > 0)
                        {
                            audioDataBase64 = parts[0].inlineData?.data ?? string.Empty;
                            mimeType = parts[0].inlineData?.mimeType ?? string.Empty;
                        }
                    }

                    if (string.IsNullOrEmpty(audioDataBase64))
                    {
                        apiResponse.error = "TTS response did not contain audio data.";
                        apiResponse.status = EAPIStatus.Error;
                        return apiResponse;
                    }

                    Debug.Log($"TTS mimeType: {mimeType}");

                    byte[] pcmBytes = Convert.FromBase64String(audioDataBase64);
                    AudioClip clip = CreateAudioClipFromPcm16(pcmBytes, GeminiSampleRate, Channels);

                    if (clip == null)
                    {
                        apiResponse.error = "Failed to create AudioClip from PCM data.";
                        apiResponse.status = EAPIStatus.Error;
                        return apiResponse;
                    }

                    apiResponse.responseObject = clip;
                    apiResponse.response = "AudioClip created successfully.";
                    apiResponse.responseType = ResponseType.AUDIO;
                    apiResponse.status = EAPIStatus.Success;

                    OnAudioRecieved?.Invoke();
                    return apiResponse;
                }
                catch (Exception e)
                {
                    apiResponse.error = e.Message;
                    apiResponse.status = EAPIStatus.Error;
                    return apiResponse;
                }
            }
        }

        private static AudioClip CreateAudioClipFromPcm16(byte[] pcmBytes, int sampleRate, int channels)
        {
            if (pcmBytes == null || pcmBytes.Length < 2)
                return null;

            int sampleCount = pcmBytes.Length / 2;
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                short sample16 = BitConverter.ToInt16(pcmBytes, i * 2);
                samples[i] = sample16 / 32768f;
            }

            int frames = sampleCount / channels;
            AudioClip clip = AudioClip.Create("GeminiTTS", frames, channels, sampleRate, false);
            clip.SetData(samples, 0);

            return clip;
        }
    }
}
