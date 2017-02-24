using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Newtonsoft.Json;
using StockMood.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace StockMood.Api
{
    public class Function
    {
        public APIGatewayProxyResponse GetTweets(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string>()
            };

            response.Headers["Access-Control-Allow-Origin"] = "*";

            context.Logger.LogLine(JsonConvert.SerializeObject(input.QueryStringParameters));

            var sortOrder = 0;
            var limit = 0;
            if (input.QueryStringParameters != null)
            {
                if (input.QueryStringParameters.ContainsKey("sortOrder"))
                {
                    if (!int.TryParse(input.QueryStringParameters["sortOrder"], out sortOrder))
                        sortOrder = 0;
                }
                if (input.QueryStringParameters.ContainsKey("limit"))
                {
                    if (!int.TryParse(input.QueryStringParameters["limit"], out limit))
                        limit = 0;
                }
            }

            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] { }).GetRemainingAsync().Result;
            if (tweets.Count > 1)
            {
                if (sortOrder > 0)
                    tweets = tweets.OrderByDescending(x => x.PopularityScore).ToList();
                else if (sortOrder < 0)
                    tweets = tweets.OrderBy(x => x.PopularityScore).ToList();

                tweets = limit > 0 ? tweets.Take(limit).ToList() : tweets.Take(100).ToList();
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Body = JsonConvert.SerializeObject(tweets);
            return response;
        }

        public APIGatewayProxyResponse GetPublicSentiment(ILambdaContext context)
        {
            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] {}).GetRemainingAsync().Result;
            dbContext.Dispose();
            context.Logger.LogLine($"Number of tweets: {tweets.Count}");
            var count = tweets.Count;
            float positive = 0;

            if (count > 0)
            {
                positive = tweets.Count(x => x.PopularityScore > 0);
                context.Logger.LogLine($"Number of positive tweets: {positive}");
            }
            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string>()
            };

            response.Headers["Access-Control-Allow-Origin"] = "*";
            response.Body = JsonConvert.SerializeObject(new PublicSentimentDto() { PublicSentiment = (float)positive / (float)count * (float)100 });
            response.StatusCode = (int) HttpStatusCode.OK;

            return response;
        }

        public APIGatewayProxyResponse GetMostTrending()
        {
            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string>()
            };

            response.Headers["Access-Control-Allow-Origin"] = "*";

            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] { }).GetRemainingAsync().Result;
            dbContext.Dispose();

            var tweet = tweets.OrderByDescending(x => Math.Abs(x.PopularityScore)).FirstOrDefault();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Body = JsonConvert.SerializeObject(tweet);
            return response;
        }

        public APIGatewayProxyResponse GetMostPopularWords()
        {
            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string>()
            };

            response.Headers["Access-Control-Allow-Origin"] = "*";

            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] { }).GetRemainingAsync().Result;
            dbContext.Dispose();

            tweets = tweets.OrderByDescending(x => Math.Abs(x.PopularityScore)).Take(100).ToList();
            var wordCounter = new Dictionary<string, int>();
            foreach (var tweet in tweets)
            {
                var words = tweet.Text.Split(" ".ToCharArray());
                foreach (var word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        var lowerWord = word.ToLower();
                        if (wordCounter.ContainsKey(lowerWord))
                            wordCounter[lowerWord] = wordCounter[lowerWord] + 1;
                        else
                            wordCounter[lowerWord] = 1;
                    }
                }
            }

            var mostPopular = wordCounter.OrderByDescending(x => x.Value).Take(10);
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Body = JsonConvert.SerializeObject(mostPopular);
            return response;
        }
    }
}
