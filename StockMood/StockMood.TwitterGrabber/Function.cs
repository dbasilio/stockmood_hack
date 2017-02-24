using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
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

            Tweet.PublishTweet(DateTime.Now.ToString());

            var tweets = Search.SearchTweets("MSFT").Take(10);

            foreach (var tweet in tweets)
            {
                context.Logger.LogLine(tweet.Text);
            }
            context.Logger.LogLine("finished running");
        }
    }
}
