using AdaptiveCards;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TwitterSentimentAnalysisBot.Model;
using TwitterSentimentAnalysisBot.Services.Abstractions;

namespace TwitterSentimentAnalysisBot.Services
{
    public class AdaptiveCardConstructorService : IAdaptiveCardConstructorService
    {
        readonly string baseUrl;

        public AdaptiveCardConstructorService(IConfiguration configuration)
        {
            baseUrl = configuration["ChartImageBaseUrl"];
        }

        public Attachment GetAnalysisCard(TwitterSentimentResponse data)
        {
            var cardBodyElements = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock
                {
                     Text = $"Number of tweets processed: {data.numberOfTweetsFound}",
                     Size = AdaptiveTextSize.Large,
                     Weight= AdaptiveTextWeight.Bolder
                },
                GetStatisticsChart(data.tweetSentimentAnalysis, data.numberOfTweetsFound)
            };

            cardBodyElements.AddRange(GetPopularTweetCards(data.mostPopularTweets));

            AdaptiveSchemaVersion defaultSchema = new(1, 0);

            AdaptiveCard card = new(defaultSchema)
            {
                Body = cardBodyElements
            };

            return CreateAdaptiveCardAttachment(card.ToJson());
        }

        IEnumerable<AdaptiveElement> GetPopularTweetCards(TweetData[] mostPopularTweets)
        {
            List<AdaptiveElement> result = new List<AdaptiveElement>();

            foreach (TweetData tweet in mostPopularTweets)
            {
                result.AddRange(GetPopularTweetCard(tweet));
            }

            return result;
        }

        IEnumerable<AdaptiveElement> GetPopularTweetCard(TweetData tweet)
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

        AdaptiveImage GetStatisticsChart(Dictionary<string, int> tweetSentimentAnalysis, int numberOfTweetsFound)
        {
            return new AdaptiveImage
            {
                Size = AdaptiveImageSize.Large,
                Url = GetPieChartUrl(tweetSentimentAnalysis, numberOfTweetsFound)
            };
        }

        Uri GetPieChartUrl(Dictionary<string, int> tweetSentimentAnalysis, int numberOfTweetsFound)
        {
            string seriesNames = string.Join('|', tweetSentimentAnalysis.Keys);
            string percentages = string.Empty;

            foreach (var val in tweetSentimentAnalysis.Values)
            {
                percentages += $"{Math.Round((float)val * 100 / numberOfTweetsFound)},";
            }

            percentages = percentages.Remove(percentages.Length - 1);

            return new Uri(string.Format(baseUrl, percentages, seriesNames));
        }

        Attachment CreateAdaptiveCardAttachment(string jsonData)
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