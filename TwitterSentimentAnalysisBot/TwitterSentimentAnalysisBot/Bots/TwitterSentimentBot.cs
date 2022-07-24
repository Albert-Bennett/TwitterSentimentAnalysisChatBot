// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.15.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwitterSentimentAnalysisBot.Services.Abstractions;

namespace TwitterSentimentAnalysisBot.Bots
{
    public class TwitterSentimentBot : ActivityHandler
    {
        readonly ITwitterAnalysisAppService _twitterAnalysisAppService;
        readonly IAdaptiveCardConstructorService _adaptiveCardConstructorService;

        public TwitterSentimentBot(ITwitterAnalysisAppService twitterAnalysisAppService, 
            IAdaptiveCardConstructorService adaptiveCardConstructorService)
        {
            _twitterAnalysisAppService = twitterAnalysisAppService;
            _adaptiveCardConstructorService = adaptiveCardConstructorService;   
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userInput = turnContext.Activity.Text;
            var response = await _twitterAnalysisAppService.GetSentimentAnalysisForTweets(userInput, 2);

            if (response == null)
            {
                string errorText = "There was an issue contacting the function app";
                await turnContext.SendActivityAsync(MessageFactory.Text(errorText, errorText), cancellationToken);
            }
            else
            {
                var adaptiveCard = _adaptiveCardConstructorService.GetAnalysisCard(response);
                await turnContext.SendActivityAsync(MessageFactory.Attachment(adaptiveCard));
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
