using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SagoApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent(); // Kopplar ihop XAML och code-behind
        }

        private async void OnGenerateStoryClicked(object sender, EventArgs e)
        {
            // Hämta texten från Editor och Entry
            string prompt = PromptEditor.Text;
            string names = NamesEntry.Text;

            // Skapa en sammansatt prompt med barnens namn
            string fullPrompt = $"Skriv en saga för barn. Sagan ska handla om: {prompt}. Inkludera följande namn: {names}.";

            // Visa en enkel indikation
            StoryEditor.Text = "Genererar saga...";

            // Anropa API:t för att generera sagan
            string generatedStory = await GenerateStoryAsync(fullPrompt);

            // Visa den genererade sagan
            StoryEditor.Text = generatedStory;
        }

        private async Task<string> GenerateStoryAsync(string prompt)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Ersätt "YOUR_OPENAI_API_KEY" med din riktiga API-nyckel
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "YOUR_OPENAI_API_KEY");

                    var requestData = new
                    {
                        model = "text-davinci-003",
                        prompt = prompt,
                        max_tokens = 500
                    };

                    var json = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://api.openai.com/v1/completions", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                        return result.choices[0].text.ToString();
                    }
                    else
                    {
                        return "Kunde inte generera sagan. Försök igen senare.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Ett fel uppstod: {ex.Message}";
            }
        }
    }
}
