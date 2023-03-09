using ChatGPT.Json;
using ChatGPT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Entities
{
    public class Conversation
    {
        //This class will contain login that defines the lifespan of a conversation and any settings changes made to the header message

        public ChatGPTApiService apiService;

        public List<ChatMessage> messages = new List<ChatMessage>();
        
        public Conversation() 
        {
            string Prompt = "You are a chatbot designed to give helpful answers. When the response provided by the user indicates the end of the conversation you will finish your response with '/0'";

            apiService = new ChatGPTApiService();

            messages.Add(new ChatMessage
            {
                Role = "system",
                Content = Prompt,
            });
        }

        public async Task<string> SendMessage(string message) 
        {
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = message,
            });

            var response = await apiService.GetResponseDataAsync(messages.ToArray());

            Console.WriteLine($"Total Tokens Used - {response.Usage.TotalTokens}");

            messages.Add(new ChatMessage
            {
                Role = "assistant",
                Content = response.Choices[0].Message.Content,
            });

            return response.Choices[0].Message.Content;
        }
    }
}
