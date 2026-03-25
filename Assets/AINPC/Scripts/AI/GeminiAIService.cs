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
    public class GeminiAIService : ILLMService
    {
        #region Gemini AI Class

        [Serializable]
        private class GeminiRequest
        {
            public List<Content> contents;
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

        private GeminiAISetting _geminiAISetting = null;
        public UnityEvent<string> OnResponseReceived = new();

        public async Task<APIResult> GetResponseAsync(string prompt)
        {
            APIResult apiResult = new();

            // TODO : handle this better, maybe initialize if null
            if (_geminiAISetting == null)
            {
                _geminiAISetting = Resources.Load("Data/GeminiAICreds") as GeminiAISetting;

                if (_geminiAISetting == null)
                {
                    Debug.LogError("Error Gemini API Creds are unavailable.");

                    apiResult.Error = "Error Gemini API Creds are unavailable.";
                    apiResult.Status = EAPIStatus.Error;

                    return apiResult;
                }
            }

            Debug.Log("Prompt Received : " + prompt);

            var requestBody = new GeminiRequest
            {
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
                }
            };

            string jsonBody = JsonUtility.ToJson(requestBody);
            string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";

            using (var request = new UnityWebRequest(url, "POST"))
            {
                byte[] jsonRaw = Encoding.UTF8.GetBytes(jsonBody);

                request.uploadHandler = new UploadHandlerRaw(jsonRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-type", "application/json");
                request.SetRequestHeader("X-goog-api-key", _geminiAISetting.apiKey);

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    apiResult.Status = EAPIStatus.Processing;
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Request Returned : " + request.error + request.responseCode);
                    OnResponseReceived?.Invoke(request.error);

                    apiResult.Error = request.error;
                    apiResult.Status = EAPIStatus.Error;

                    return apiResult;
                }

                try
                {
                    string responseBody = request.downloadHandler.text;
                    GeminiResponse geminiResponse = JsonUtility.FromJson<GeminiResponse>(responseBody);

                    string results = string.Empty;

                    if (geminiResponse?.candidates != null && geminiResponse.candidates.Count > 0)
                    {
                        var parts = geminiResponse.candidates[0].content.parts;

                        if (parts != null && parts.Count > 0)
                            results = parts[0].text;
                    }

                    OnResponseReceived.Invoke(results);
                    apiResult.Response = results;
                    apiResult.Status = EAPIStatus.Success;

                    return apiResult;
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception Caught : " + e);
                    throw;
                }
            }
        }
    }
}