using FrontEndAudToText.Models;
using Newtonsoft.Json;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FrontEndAudToText.Services
{
    public class CommunicationServiceWithApi
    {
        private readonly HttpClient _httpClient;

        public CommunicationServiceWithApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetTextFromYouTube(string ytUrl)
        {
            try
            {
                //https://localhost:7282/api/Handler/TransYT
                HttpRequestMessage requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7282/api/Handler/TransYT"),
                    Content = new StringContent(JsonSerializer.Serialize(ytUrl), Encoding.UTF8, "application/json")
                };
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode)
                {
                    return "some err";
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return new string("Bad Url Ex: " + ex.Message);
            }

        }
        public async Task<List<WordResult>?> GetSearchResults(string word)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7282/api/Handler/SearchWord"),
                Content = new StringContent(JsonSerializer.Serialize(word), Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            string resul = await response.Content.ReadAsStringAsync();
            List<WordResult>? lwr = JsonConvert.DeserializeObject<List<WordResult>>(resul);
            return lwr;
            //SearchWord
        }
    }
}
