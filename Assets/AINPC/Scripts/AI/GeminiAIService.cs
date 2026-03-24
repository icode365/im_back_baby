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
    public class Gemini : ILLMService
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

        private GeminiAICreds _geminiAICreds = new();
        public UnityEvent<string> OnResponseReceived = new();

        public async Task<string> GetResponse(string prompt)
        {
            // TODO : handle this better, maybe initialize if null
            if (_geminiAICreds == null)
            {
                Debug.LogError("Error Gemini API Creds are unavailable.");
                return null;
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
                request.SetRequestHeader("X-goog-api-key", _geminiAICreds.apiKey);

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Request Returned : " + request.error + request.responseCode);
                    OnResponseReceived?.Invoke(request.error);
                    return $"Error : {request.error}";
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
                    return results;
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