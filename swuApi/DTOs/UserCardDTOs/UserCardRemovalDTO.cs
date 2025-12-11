namespace swuApi.UserCardDTOs
{
    public class UserCardRemovalDTO
    {
        public int UserId { get; set; }
        public int CardId { get; set; }
        public int CopiesToRemove { get; set; }
    }
}