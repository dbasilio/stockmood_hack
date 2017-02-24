using System;
using Amazon.DynamoDBv2.DataModel;

namespace StockMood.Models
{
    [DynamoDBTable("StockMood_Tweets")]
    public class TweetDto
    {
        [DynamoDBHashKey]
        public string TweetId { get; set; }
        public string Text { get; set; }
        public UserDto User { get; set; }
        public int NumberOfRetweets { get; set; }
        public int NumberOfLikes { get; set; }
        public DateTime DateCreated { get; set; }
        public string Permalink { get; set; }
        public float Score { get; set; }
        public float Magnitude { get; set; }

        public override string ToString()
        {
            return $"Text: {Text}, NumberOfRetweets: {NumberOfRetweets}, NumberOfLikes: {NumberOfLikes}, User Details: {User}";
        }
    }
}
