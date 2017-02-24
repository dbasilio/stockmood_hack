using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using StockMood.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace StockMood.Api
{
    public class Function
    {
        public IEnumerable<TweetDto> GetTweets(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            int sortOrder;
            if (!int.TryParse(input.QueryStringParameters["sortOrder"], out sortOrder))
                sortOrder = 0;

            int limit;
            if (!int.TryParse(input.QueryStringParameters["limit"], out limit))
                limit = 0;

            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] { }).GetRemainingAsync().Result;
            if (tweets.Count < 1)
                return new TweetDto[] {};

            if (sortOrder > 0)
                tweets = tweets.OrderByDescending(x => x.PopularityScore).ToList();
            else if (sortOrder < 0)
                tweets = tweets.OrderBy(x => x.PopularityScore).ToList();
            tweets = limit > 0 ? tweets.Take(limit).ToList() : tweets.Take(100).ToList();

            return tweets;
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

        public TweetDto GetMostTrending()
        {
            var dynamoDbClient =
                new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
                    "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            var dbContext = new DynamoDBContext(dynamoDbClient);

            var tweets = dbContext.ScanAsync<TweetDto>(new ScanCondition[] { }).GetRemainingAsync().Result;
            dbContext.Dispose();

            return tweets.OrderByDescending(x => Math.Abs(x.PopularityScore)).FirstOrDefault();
        }
    }
}
