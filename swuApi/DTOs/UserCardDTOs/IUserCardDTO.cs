namespace swuApi.UserCardDTOs
{
    public abstract class IUserCardDTO
    {
        public int UserId { get; set; }
        public int CardId { get; set; }
        public int Copies { get; set; } = 1;
        public DateTime? DateAdded { get; set; } = null;
        public bool IsFavorite { get; set; } = false;
    }
}