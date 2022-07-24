using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TwitterSentimentAnalysisBot.Model;
using TwitterSentimentAnalysisBot.Services.Abstractions;

namespace TwitterSentimentAnalysisBot.Services
{
    public class TwitterAnalysisAppService : ITwitterAnalysisAppService
    {
        readonly IHttpClientFactory _httpClientFactory;
        readonly string baseUrl;

        public TwitterAnalysisAppService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;

            baseUrl = configuration["TwitterSentimentAnalysisBaseUrl"];
        }

        public async Task<TwitterSentimentResponse> GetSentimentAnalysisForTweets(string searchTerm, int? maxResults = 10)
        {
            string url = $"{baseUrl}?hashtags={searchTerm}&max_tweets={maxResults}";

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();

                    return await JsonSerializer.DeserializeAsync<TwitterSentimentResponse>(contentStream);
                }
            }

            return null;
        }
    }
}
