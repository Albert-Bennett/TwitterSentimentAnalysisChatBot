using Microsoft.Bot.Schema;
using TwitterSentimentAnalysisBot.Model;

namespace TwitterSentimentAnalysisBot.Services.Abstractions
{
    public interface IAdaptiveCardConstructorService
    {
        public Attachment GetAnalysisCard(TwitterSentimentResponse data);
    }
}
