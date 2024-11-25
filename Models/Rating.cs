namespace LetterBoxd.Models
{
    public class Rating
    {
        public int Id { get;set; }
        public string Username { get; set; }
        public int Score { get;set; }
        public int MovieId { get;set; }
        public Movie Movie { get;set; }
    }
} 