using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace SagoApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent(); // Kopplar ihop XAML och code-behind
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, EventArgs e)
        {
            // Om du vill använda Secure Storage för andra inställningar kan du göra det här.
            // I den här backend-lösningen hanteras API-nyckeln centralt, så inget mer behövs här.
        }

        private async void OnGenerateStoryClicked(object sender, EventArgs e)
        {
            // Hämta texten från Editor och Entry
            string prompt = PromptEditor.Text;
            string names = NamesEntry.Text;

            // Skapa en sammansatt prompt med barnens namn
            string fullPrompt = $"Skriv en saga för barn. Sagan ska handla om: {prompt}. Inkludera följande namn: {names}.";

            // Visa en indikation på att sagan skapas
            StoryEditor.Text = "Skapar er saga...";

            // Anropa backend‑API:et för att generera sagan
            string generatedStory = await GenerateStoryFromBackendAsync(fullPrompt, names);

            // Visa den genererade sagan
            StoryEditor.Text = generatedStory;
        }

        private async Task<string> GenerateStoryFromBackendAsync(string prompt, string names)
        {
            try
            {
                // Skapa request‑objektet
                var requestObject = new StoryRequest
                {
                    Prompt = prompt,
                    Names = names
                };

                // Serialisera request‑objektet till JSON
                string jsonRequest = JsonSerializer.Serialize(requestObject);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Använd HttpClient. (För Android‑emulatorn, använd 10.0.2.2 istället för localhost.)
                using HttpClient client = new HttpClient();

                // Bestäm backend‑URL beroende på plattform
                // Använd samma publika URL oavsett plattform
                string backendUrl = "https://sagoapp-api-proxy20250402112754.azurewebsites.net/api/story/generate";

                //string backendUrl;
                //if (DeviceInfo.Platform == DevicePlatform.Android)
                //{
                //    backendUrl = "https://10.0.2.2:7232/api/Story/generate";
                //}
                //else
                //{
                //    backendUrl = "https://sagoapp-api-proxy20250402112754.azurewebsites.net/api/Story/generate";
                //}

                HttpResponseMessage response = await client.PostAsync(backendUrl, content);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var storyResponse = JsonSerializer.Deserialize<StoryResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return storyResponse?.Story ?? "Inget svar mottaget.";
            }
            catch (Exception ex)
            {
                return $"Ett fel uppstod: {ex.Message}";
            }
        }

    }

    // Modell för inkommande request
    public class StoryRequest
    {
        public string Prompt { get; set; }
        public string Names { get; set; }
    }

    // Modell för response
    public class StoryResponse
    {
        public string Story { get; set; }
    }
}



//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Microsoft.Maui.Controls;
//using System.Collections.Generic; // Lägg till för IAsyncEnumerable
//using OpenAI;
//using OpenAI.Chat;
//using Microsoft.Maui.Storage;


//namespace SagoApp
//{
//    public partial class MainPage : ContentPage
//    {
//        private OpenAIClient _chatGptClient;

//        public MainPage()
//        {
//            InitializeComponent(); // Kopplar ihop XAML och code-behind
//            this.Loaded += MainPage_Loaded;
//        }

//        private async void MainPage_Loaded(object sender, EventArgs e)
//        {
//            // Försök hämta API-nyckeln från Secure Storage
//            var openAiKey = await SecureStorage.Default.GetAsync("OPENAI_API_KEY");
//            if (string.IsNullOrWhiteSpace(openAiKey))
//            {
//                await DisplayAlert("Fel", "API-nyckeln saknas i Secure Storage. Var god konfigurera API-nyckeln.", "OK");
//                return;
//            }
//            _chatGptClient = new OpenAIClient(openAiKey);
//        }



//        private async void OnGenerateStoryClicked(object sender, EventArgs e)
//        {
//            // Hämta texten från Editor och Entry
//            string prompt = PromptEditor.Text;
//            string names = NamesEntry.Text;

//            // Skapa en sammansatt prompt med barnens namn
//            string fullPrompt = $"Skriv en saga för barn. Sagan ska handla om: {prompt}. Inkludera följande namn: {names}.";

//            // Visa en enkel indikation
//            StoryEditor.Text = "Skapar er saga...";

//            // Anropa API:t för att generera sagan
//            string generatedStory = await GenerateStoryAsync(fullPrompt);

//            // Visa den genererade sagan
//            StoryEditor.Text = generatedStory;
//        }

//        private async Task<string> GenerateStoryAsync(string prompt)
//        {
//            try
//            {
//                // Använd en modell som är tillgänglig för ditt konto, t.ex. "gpt-3.5-turbo-16k"
//                var chatClient = _chatGptClient.GetChatClient("gpt-4o-mini");


//                IAsyncEnumerable<StreamingChatCompletionUpdate> updates = chatClient.CompleteChatStreamingAsync(prompt);
//                StringWriter responseWriter = new StringWriter();

//                await foreach (StreamingChatCompletionUpdate update in updates)
//                {
//                    foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
//                    {
//                        responseWriter.Write(updatePart.Text);
//                    }
//                }

//                return responseWriter.ToString();
//            }
//            catch (Exception ex)
//            {
//                return $"Ett fel uppstod: {ex.Message}";
//            }
//        }

//        //LÄGGA TILL API-nyckel
//        private async void OnSaveApiKeyClicked(object sender, EventArgs e)
//        {
//            // Hämta texten från Entry-fältet
//            string apiKey = ApiKeyEntry.Text;
//            if (string.IsNullOrWhiteSpace(apiKey))
//            {
//                await DisplayAlert("Fel", "Ange en giltig API-nyckel.", "OK");
//                return;
//            }

//            // Spara API-nyckeln i Secure Storage
//            try
//            {
//                await SecureStorage.Default.SetAsync("OPENAI_API_KEY", apiKey);
//                await DisplayAlert("Sparat", "API-nyckeln har sparats säkert.", "OK");
//            }
//            catch (Exception ex)
//            {
//                await DisplayAlert("Fel", $"Kunde inte spara API-nyckeln: {ex.Message}", "OK");
//            }
//        }

//        //private void MainPage_Loaded(object sender, EventArgs e)
//        //{
//        //    // Hämta API-nyckeln från miljövariabeln
//        //    var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
//        //    if (string.IsNullOrWhiteSpace(openAiKey))
//        //    {
//        //        DisplayAlert("Fel", "API-nyckeln saknas. Var god sätt miljövariabeln OPENAI_API_KEY.", "OK");
//        //        return;
//        //    }
//        //    _chatGptClient = new OpenAIClient(openAiKey);
//        //}
//    }
//}
