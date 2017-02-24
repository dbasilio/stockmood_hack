using Amazon.DynamoDBv2.DataModel;

namespace StockMood.Models
{
    [DynamoDBTable("StockMood_SearchKeywords")]
    public class SearchKeywordsDto
    {
        [DynamoDBHashKey]
        public string Keyword { get; set; }
    }
}
