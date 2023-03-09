using ChatGPT.Entities;
using Microsoft.CognitiveServices.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace MicKeywordDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            ListenForActivation();
            Console.ReadLine();
        }

        public static void ListenForActivation() 
        {
            // Create a new SpeechRecognitionEngine instance
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

            // Add a grammar that includes the keyword you want to listen for
            GrammarBuilder grammarBuilder = new GrammarBuilder("jarvis");
            System.Speech.Recognition.Grammar grammar = new System.Speech.Recognition.Grammar(grammarBuilder);
            recognizer.LoadGrammar(grammar);
            recognizer.LoadGrammar(new DictationGrammar());


            // Attach an event handler to the SpeechRecognized event
            recognizer.SpeechRecognized += SpeechRecognizedHandler;

            // Start the recognition engine
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            Console.WriteLine("Listening for 'jarvis'...");
        }

        static async void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;

            if (recognizedText.ToLower().Contains("jarvis"))
            {
                Console.WriteLine("Keyword detected: 'Jarvis'");
                //Start new conversation
                await HoldConversation();
            }
        }

        static async Task HoldConversation() 
        {
            Conversation newConversation = new Conversation();

            do
            {
                var config = SpeechConfig.FromSubscription(Environment.GetEnvironmentVariable("JarvisLanguageKey", EnvironmentVariableTarget.User), "westus");

                using (var recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(config))
                {
                    Console.WriteLine("Say something...");

                    var result = await recognizer.RecognizeOnceAsync();

                    // Checks result.
                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine($"Recognized: {result.Text}");

                        await SpeakResponseFromGptAsync(result.Text, newConversation);
                    }
                    else if (result.Reason == ResultReason.NoMatch)
                    {
                        Console.Write($"NOMATCH: Speech could not be recognized.");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = CancellationDetails.FromResult(result);

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            throw new Exception($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        }

                        throw new Exception($"CANCELED: Reason={cancellation.Reason}");
                    }
                }
            } while (newConversation.messages.Last().Content.EndsWith("/0") == false);
        }

        private static async Task SpeakResponseFromGptAsync(string text, Conversation conversation)
        {
            string response = await conversation.SendMessage(text);

            Console.WriteLine(response);

            var synthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            char[] c = { '0', '/' };
            synthesizer.Speak(response.TrimEnd(c));
        }
    }
}
