namespace swuApi.Models
{
    public class UserCard
    {
        public int Id {get; set;}
        public int UserId { get; set; }
        public int CardId { get; set; }
        public int Copies { get; set; } = 1;
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public bool IsFavorite { get; set; } = false;
        public User? User { get; set; }
        public Card? Card { get; set; }

        public UserCard()
        {
            DateAdded = DateTime.UtcNow;
        }
    }
}