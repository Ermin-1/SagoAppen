using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Devices; // För plattformsinformation

namespace SagoApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
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

                // Använd samma URL oavsett plattform, eftersom backend nu är publicerad
                string backendUrl = "https://sagoapp-api-proxy20250402112754.azurewebsites.net/api/story/generate";

                using HttpClient client = new HttpClient();
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

        // Eventhandler för fördefinierade promptar
        private void OnPresetPromptClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string preset)
            {
                // Sätt promptfältet med den fördefinierade texten
                PromptEditor.Text = preset;
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
