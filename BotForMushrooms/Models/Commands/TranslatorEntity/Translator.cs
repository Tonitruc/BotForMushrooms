using Newtonsoft.Json.Linq;

namespace BotForMushrooms.Models.Commands.TranslatorEntity
{
    public static class Translator
    {
        private readonly static string ApiUrl = "https://ftapi.pythonanywhere.com/translate";

        public static async Task<string?> TranslateText(string text, string sourseLanguage = "en", string destinationLanguage = "ru")
        {
            string url = $"{ApiUrl}?sl={sourseLanguage}&dl={destinationLanguage}&text={text}";

            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(responseBody);

                return json["destination-text"].ToString();

            }
            else
            {
                return null;
            }
        }
    }
}
