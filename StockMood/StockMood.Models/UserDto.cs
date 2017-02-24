namespace StockMood.Models
{
    public class UserDto
    {
        public string ScreenName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public int NumberOfFollowers { get; set; }

        public override string ToString()
        {
            return
                $"ScreenName: {ScreenName}, UserName: {UserName}, EmailAddress: {EmailAddress}, NumberOfFollowers: {NumberOfFollowers}";
        }
    }
}
