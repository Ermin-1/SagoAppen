using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Collections.Generic; // Lägg till för IAsyncEnumerable
using OpenAI;
using OpenAI.Chat;

namespace SagoApp
{
    public partial class MainPage : ContentPage
    {
        private OpenAIClient _chatGptClient;

        public MainPage()
        {
            InitializeComponent(); // Kopplar ihop XAML och code-behind
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, EventArgs e)
        {
            // Hämta API-nyckeln från miljövariabeln
            var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrWhiteSpace(openAiKey))
            {
                DisplayAlert("Fel", "API-nyckeln saknas. Var god sätt miljövariabeln OPENAI_API_KEY.", "OK");
                return;
            }
            _chatGptClient = new OpenAIClient(openAiKey);
        }

        private async void OnGenerateStoryClicked(object sender, EventArgs e)
        {
            // Hämta texten från Editor och Entry
            string prompt = PromptEditor.Text;
            string names = NamesEntry.Text;

            // Skapa en sammansatt prompt med barnens namn
            string fullPrompt = $"Skriv en saga för barn. Sagan ska handla om: {prompt}. Inkludera följande namn: {names}.";

            // Visa en enkel indikation
            StoryEditor.Text = "Skapar er saga...";

            // Anropa API:t för att generera sagan
            string generatedStory = await GenerateStoryAsync(fullPrompt);

            // Visa den genererade sagan
            StoryEditor.Text = generatedStory;
        }

        private async Task<string> GenerateStoryAsync(string prompt)
        {
            try
            {
                // Använd en modell som är tillgänglig för ditt konto, t.ex. "gpt-3.5-turbo-16k"
                var chatClient = _chatGptClient.GetChatClient("gpt-4o-mini");

          
                IAsyncEnumerable<StreamingChatCompletionUpdate> updates = chatClient.CompleteChatStreamingAsync(prompt);
                StringWriter responseWriter = new StringWriter();

                await foreach (StreamingChatCompletionUpdate update in updates)
                {
                    foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                    {
                        responseWriter.Write(updatePart.Text);
                    }
                }

                return responseWriter.ToString();
            }
            catch (Exception ex)
            {
                return $"Ett fel uppstod: {ex.Message}";
            }
        }
    }
}
