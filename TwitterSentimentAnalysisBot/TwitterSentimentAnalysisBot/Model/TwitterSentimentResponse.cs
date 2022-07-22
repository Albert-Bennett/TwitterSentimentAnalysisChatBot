using System.Collections.Generic;

namespace TwitterSentimentAnalysisBot.Model
{
    public class TwitterSentimentResponse
    {
        public TweetData[] mostPopularTweets { get; set; }
        public int numberOfTweetsFound { get; set; }
        public Dictionary<string, int> tweetSentimentAnalysis { get; set; }
    }
}
