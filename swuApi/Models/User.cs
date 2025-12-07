namespace swuApi.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public decimal TotalCollectionValue { get; set; } = 0.00M; 
        public ICollection<UserCard>? Inventory { get; set; }

        public User()
        {
            Inventory = new List<UserCard>();
        }
    }
}