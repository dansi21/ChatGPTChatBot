using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatGPT.Json
{
    public class ChatRequestBody
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("messages")]
        public ChatMessage[]? Messages { get; set; }

        [JsonPropertyName("temperature")]
        public decimal Temperature { get; set; } = 1;

        [JsonPropertyName("top_p")]
        public decimal TopP { get; set; } = 1;

        [JsonPropertyName("n")]
        public int N { get; set; } = 1;

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("stop")]
        public string? Stop { get; set; }

        [JsonPropertyName("max_tokens")]
        public int Max_Tokens { get; set; } = 16;

        [JsonPropertyName("presence_penalty")]
        public decimal Presence_Penalty { get; set; }

        [JsonPropertyName("frequency_penalty")]
        public decimal Frequency_Penalty { get; set; }

        [JsonPropertyName("logit_bias")]
        public Dictionary<string, decimal>? Logit_Bias { get; set; }

        [JsonPropertyName("user")]
        public string? User { get; set; }
    }

    public class ChatChoice
    {
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("logprobs")]
        public object? Logprobs { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    public class ChatError
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("param")]
        public object? Param { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }

    public class ChatMessage
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    public abstract class ChatResponse
    {
    }

    public class ChatResponseError : ChatResponse
    {
        [JsonPropertyName("error")]
        public ChatError? Error { get; set; }
    }

    public class ChatResponseSuccess : ChatResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public ChatChoice[]? Choices { get; set; }

        [JsonPropertyName("usage")]
        public ChatUsage? Usage { get; set; }
    }

    public class ChatUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
