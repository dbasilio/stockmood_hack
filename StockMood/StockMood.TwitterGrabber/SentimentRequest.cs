using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace StockMood.TwitterGrabber
{
    public class SentimentRequest
    {
        [JsonProperty("document")]
        public SentimentDocument Document { get; set; }
        [JsonProperty("encodingType")]
        public string EncodingType { get; set; }
    }

    public class SentimentDocument
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("type")]
        public string @Type { get; set; }
    }
}
