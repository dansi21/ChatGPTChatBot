using ChatGPT.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatGPT.Services
{
    public class ChatGPTApiService
    {
        private static readonly HttpClient s_client = new();

        private static readonly ChatJsonContext s_serializerContext = new(
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true
            });

        private static string GetRequestBodyJson(ChatMessage[] incomingMessages)
        {
            // Set up the request body
            var requestBody = new ChatRequestBody
            {
                Model = "gpt-3.5-turbo",
                Messages = incomingMessages,
                Max_Tokens = 256,
                Temperature = 0.7m,
                TopP = 1,
                N = 1,
                Stream = false,
                Stop = null,
                Frequency_Penalty = 0.0m,
                Presence_Penalty = 0.0m,
                User = null
            };

            // Serialize the request body to JSON using the JsonSerializer.
            return JsonSerializer.Serialize(requestBody, s_serializerContext.ChatRequestBody);
        }

        private static async Task<ChatResponse?> SendApiRequestAsync(string apiUrl, string apiKey, string requestBodyJson)
        {
            // Create a new HttpClient for making the API request

            // Set the API key in the request headers
            if (s_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                s_client.DefaultRequestHeaders.Remove("Authorization");
            }
            s_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            // Create a new StringContent object with the JSON payload and the correct content type
            var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

            // Send the API request and get the response
            var response = await s_client.PostAsync(apiUrl, content);

            // Deserialize the response
            var responseBody = await response.Content.ReadAsStringAsync();

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.TooManyRequests:
                case HttpStatusCode.InternalServerError:
                    {
                        return JsonSerializer.Deserialize(responseBody, s_serializerContext.ChatResponseError);
                    }
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            // Return the response data
            return JsonSerializer.Deserialize(responseBody, s_serializerContext.ChatResponseSuccess);
        }

        public async Task<ChatResponseSuccess> GetResponseDataAsync(ChatMessage[] incomingMessages)
        {
            // Set up the API URL and API key
            var apiUrl = "https://api.openai.com/v1/chat/completions";
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey", EnvironmentVariableTarget.User);
            if (apiKey is null)
            {
                return null;
            }

            // Get the request body JSON
            var requestBodyJson = GetRequestBodyJson(incomingMessages);

            // Send the API request and get the response data
            return (ChatResponseSuccess)await SendApiRequestAsync(apiUrl, apiKey, requestBodyJson);
        }
    }
}
