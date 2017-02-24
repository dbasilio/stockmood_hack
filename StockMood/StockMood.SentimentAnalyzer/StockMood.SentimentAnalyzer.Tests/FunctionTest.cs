using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using StockMood.SentimentAnalyzer;

namespace StockMood.SentimentAnalyzer.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToUpperFunction()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            function.FunctionHandler(context);

            //Assert.Equal("HELLO WORLD", upperCase);
        }
    }
}
