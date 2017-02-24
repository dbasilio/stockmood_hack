using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using StockMood.Models;
using Tweetinvi;
using Tweetinvi.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace StockMood.TwitterGrabber
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(ILambdaContext context)
        {
            ITwitterCredentials creds = new TwitterCredentials("LeWMQwaGHAvkiGDzOaCwWnqDg",
                "O9huoRD17SbQlkHDaTtvOsJ2g3qoMXr9n9qaLKNKGesVp2FnOZ",
                "835155580037775360-k78JNWd4QiH3JVAqgpzzEBFgNYsYfp8", "PMRySX5hG2eTzilbK9ZIrVWQjs8fVZzfqvkD8A1rc7emP");
            Auth.SetCredentials(creds);

            var tweets = Search.SearchTweets("MSFT").Take(10);

            var tweetDtoList = new List<TweetDto>();
            foreach (var tweet in tweets)
            {
                var tweetDto = new TweetDto
                {
                    Text = tweet.FullText,
                    NumberOfRetweets = tweet.RetweetCount,
                    NumberOfLikes = tweet.FavoriteCount,
                    DateCreated = tweet.CreatedAt,
                    User = new UserDto
                    {
                        ScreenName = tweet.TweetDTO.CreatedBy.ScreenName,
                        EmailAddress = tweet.TweetDTO.CreatedBy.Email,
                        UserName = tweet.TweetDTO.CreatedBy.Name,
                        NumberOfFollowers = tweet.TweetDTO.CreatedBy.FollowersCount
                    }
                };
                tweetDtoList.Add(tweetDto);
                context.Logger.LogLine(tweetDto.ToString());
            }
            context.Logger.LogLine("finished running");
        }
    }
}
