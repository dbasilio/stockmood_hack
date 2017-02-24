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

            context.Logger.LogLine("1");
            var sortOrder = 0;
            var limit = 0;
            if (input.QueryStringParameters != null)
            {
                if (!int.TryParse(input.QueryStringParameters["sortOrder"], out sortOrder))
                    sortOrder = 0;

                context.Logger.LogLine("2");

                if (!int.TryParse(input.QueryStringParameters["limit"], out limit))
                    limit = 0;
            }

            context.Logger.LogLine("3");
            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] { }).GetRemainingAsync().Result;
            if (tweets.Count > 1)
            {
                context.Logger.LogLine("4");
                if (sortOrder > 0)
                    tweets = tweets.OrderByDescending(x => x.PopularityScore).ToList();
                else if (sortOrder < 0)
                    tweets = tweets.OrderBy(x => x.PopularityScore).ToList();

                context.Logger.LogLine("5");
                tweets = limit > 0 ? tweets.Take(limit).ToList() : tweets.Take(100).ToList();

                context.Logger.LogLine("6");
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Body = JsonConvert.SerializeObject(tweets);
            return response;
        }

        public int GetPublicSentiment()
        {
            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] {}).GetRemainingAsync().Result;
            dbContext.Dispose();
            
            var count = tweets.Count;
            if (count == 0)
                return 0;

            var positive = tweets.Count(x => x.PopularityScore > 0);
            return positive / count * 100;
        }

        public string GetMostTrending()
        {
            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] { }).GetRemainingAsync().Result;
            dbContext.Dispose();

            return
                JsonConvert.SerializeObject(tweets.OrderByDescending(x => Math.Abs(x.PopularityScore)).FirstOrDefault());
        }
    }
}
