using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TwitterSentimentAnalysisBot.Model;

namespace TwitterSentimentAnalysisBot.Helpers
{
    public class AdaptiveCardConstructor
    {
        public static Attachment GetAnalysisCard(TwitterSentimentResponse data)
        {
            var cardBodyElements = new List<AdaptiveElement>
            {
                GetStatisticsElement(data.tweetSentimentAnalysis, data.numberOfTweetsFound)
            };

            cardBodyElements.AddRange(GetPopularTweetCards(data.mostPopularTweets));

            AdaptiveSchemaVersion defaultSchema = new(1, 0);

            AdaptiveCard card = new(defaultSchema)
            {
                Body = cardBodyElements
            };

            return CreateAdaptiveCardAttachment(card.ToJson());
        }

        static IEnumerable<AdaptiveElement> GetPopularTweetCards(TweetData[] mostPopularTweets)
        {
            List<AdaptiveElement> result = new List<AdaptiveElement>();

            foreach (TweetData tweet in mostPopularTweets)
            {
                result.AddRange(GetPopularTweetCard(tweet));
            }

            return result;
        }

        private static IEnumerable<AdaptiveElement> GetPopularTweetCard(TweetData tweet)
        {
            return new List<AdaptiveElement>
            {
                new AdaptiveTextBlock
                {
                        Size = AdaptiveTextSize.Medium,
                        Weight = AdaptiveTextWeight.Bolder,
                        Text = tweet.text,
                        Wrap = true
                },
                new AdaptiveColumnSet
                {
                    Columns = new List<AdaptiveColumn>
                    {
                        new AdaptiveColumn
                        {
                            Items = new List<AdaptiveElement>
                            {
                                new AdaptiveTextBlock
                                {
                                    Spacing = AdaptiveSpacing.None,
                                    IsSubtle = true,
                                    Wrap = true,
                                    Text = $"Likes: {tweet.public_metrics.like_count}"
                                },
                                new AdaptiveTextBlock
                                {
                                    Spacing = AdaptiveSpacing.None,
                                    IsSubtle = true,
                                    Wrap = true,
                                    Text = $"Retweets: {tweet.public_metrics.retweet_count}"
                                }
                            }
                        },
                        new AdaptiveColumn
                        {
                            Items = new List<AdaptiveElement>
                            {
                                new AdaptiveTextBlock
                                {
                                    Spacing = AdaptiveSpacing.None,
                                    IsSubtle = true,
                                    Wrap = true,
                                    Text = $"Replies: {tweet.public_metrics.reply_count}"
                                },
                                new AdaptiveTextBlock
                                {
                                    Spacing = AdaptiveSpacing.None,
                                    IsSubtle = true,
                                    Wrap = true,
                                    Text = $"Quote count: {tweet.public_metrics.quote_count}"
                                }
                            }
                        }
                    }
                }
            };
        }

        static AdaptiveColumnSet GetStatisticsElement(Dictionary<string, int> tweetSentimentAnalysis, int numberOfTweetsFound)
        {
            return new AdaptiveColumnSet
            {
                Columns = new List<AdaptiveColumn>
                {
                    new AdaptiveColumn
                    {
                        Items = GetCardStatistics(tweetSentimentAnalysis, numberOfTweetsFound),
                        Width = "stretch"
                    },
                    new AdaptiveColumn
                    {
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Spacing = AdaptiveSpacing.None,
                                IsSubtle = true,
                                Wrap = true,
                                Text = $"Number of Tweets found: {numberOfTweetsFound}"
                            }
                        }
                    }
                }
            };
        }

        static List<AdaptiveElement> GetCardStatistics(Dictionary<string, int> data, int numberOfTweetsFound)
        {
            List<AdaptiveElement> result = new List<AdaptiveElement>();

            foreach(string sentiment in data.Keys)
            {
                result.Add(new AdaptiveTextBlock
                {
                    Spacing = AdaptiveSpacing.None,
                    IsSubtle = true,
                    Wrap = true,
                    Text = $"{sentiment}: {(data[sentiment] * 100) / numberOfTweetsFound }%"
                });
            }

            return result; 
        }

        static Attachment CreateAdaptiveCardAttachment(string jsonData)
        {
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(jsonData),
            };

            return adaptiveCardAttachment;
        }
    }
}
