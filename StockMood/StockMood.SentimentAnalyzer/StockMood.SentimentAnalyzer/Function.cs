using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Amazon.Runtime;
using Newtonsoft.Json;
using StockMood.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace StockMood.SentimentAnalyzer
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(ILambdaContext context)
        {
            //var dynamoDbClient =
            //    new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAITP2P5WZV5HT4UYA",
            //        "xa3m+F2i9QsHckUkDz+o60RpFO71PAtRWvH+8+eK"));
            //var dbContext = new DynamoDBContext(dynamoDbClient);

            //var tweets = dbContext.ScanAsync<TweetDto>(new List<ScanCondition>()).GetNextSetAsync().Result;

            //foreach (var tweetDto in tweets)
            //{
            //    context.Logger.LogLine(tweetDto.Text);
            //}
            //context.Logger.LogLine("Sentiment analyzis success!");

        }
    }
}
