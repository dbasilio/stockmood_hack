using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using StockMood.Models;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Search = Tweetinvi.Search;

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
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            ITwitterCredentials creds = new TwitterCredentials("LeWMQwaGHAvkiGDzOaCwWnqDg",
                "O9huoRD17SbQlkHDaTtvOsJ2g3qoMXr9n9qaLKNKGesVp2FnOZ",
                "835155580037775360-k78JNWd4QiH3JVAqgpzzEBFgNYsYfp8", "PMRySX5hG2eTzilbK9ZIrVWQjs8fVZzfqvkD8A1rc7emP");
            Auth.SetCredentials(creds);

            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            var tweetDtoList = new List<TweetDto>();
            var existingTweetIds = new HashSet<string>();

            var tweetIdQuery = dbContext.ScanAsync<TweetDto>(new ScanCondition[] {});
            foreach (var existingTweet in tweetIdQuery.GetRemainingAsync().Result)
            {
                existingTweetIds.Add(existingTweet.TweetId);
            }
            var searchKeywords = dbContext.ScanAsync<SearchKeywordsDto>(new ScanCondition[] {});

            var googleString = "";

            foreach (var keyword in searchKeywords.GetRemainingAsync().Result)
            {
                var tweets =
                    Search.SearchTweets(keyword.Keyword)
                        .Where(
                            x =>
                                !x.IsRetweet && x.Language == Language.English && !existingTweetIds.Contains(x.IdStr) &&
                                x.TweetDTO.CreatedBy.FollowersCount > 500)
                        .Take(1000);
                foreach (var tweet in tweets)
                {
                    tweetDtoList.Add(new TweetDto
                    {
                        TweetId = tweet.IdStr,
                        Text = tweet.Text,
                        NumberOfRetweets = tweet.RetweetCount,
                        NumberOfLikes = tweet.FavoriteCount,
                        DateCreated = tweet.CreatedAt,
                        Permalink = tweet.Url,
                        User = new UserDto
                        {
                            ScreenName = tweet.TweetDTO.CreatedBy.ScreenName,
                            EmailAddress = tweet.TweetDTO.CreatedBy.Email,
                            UserName = tweet.TweetDTO.CreatedBy.Name,
                            NumberOfFollowers = tweet.TweetDTO.CreatedBy.FollowersCount
                        }
                    });
                    googleString +=
                        $"|||| {tweet.IdStr} {new string(tweet.FullText.Where(c => !char.IsPunctuation(c)).ToArray())}";
                }
            }

            
            var batchWrite = dbContext.CreateBatchWrite<TweetDto>();
            batchWrite.AddPutItems(tweetDtoList);
            batchWrite.ExecuteAsync();
            context.Logger.LogLine("finished running");
        }
    }
}
