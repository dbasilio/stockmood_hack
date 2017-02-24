using System;

namespace StockMood.Models
{
    public class TweetDto
    {
        public string Text { get; set; }
        public UserDto User { get; set; }
        public int NumberOfRetweets { get; set; }
        public int NumberOfLikes { get; set; }
        public DateTime DateCreated { get; set; }
        public override string ToString()
        {
            return $"Text: {Text}, NumberOfRetweets: {NumberOfRetweets}, NumberOfLikes: {NumberOfLikes}, User Details: {User}";
        }
    }
}
