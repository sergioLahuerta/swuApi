namespace swuApi.UserCardDTOs
{
    public class UserCardGetAllDTO : IUserCardDTO
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? CardName { get; set; }
    }
}