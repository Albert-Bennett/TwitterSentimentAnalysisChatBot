using System.Threading.Tasks;
using TwitterSentimentAnalysisBot.Model;

namespace TwitterSentimentAnalysisBot.Services.Abstractions
{
    public interface ITwitterAnalysisAppService
    {
        Task<TwitterSentimentResponse> GetSentimentAnalysisForTweets(string searchTerm, int? maxResults = 10);
    }
}