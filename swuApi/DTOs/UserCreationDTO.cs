namespace swuApi.DTOs
{
    public class UserCreationDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = default;
        public decimal TotalCollectionValue { get; set; } = 0.00M; 
        public bool IsActive { get; set; } = true;
    }
}