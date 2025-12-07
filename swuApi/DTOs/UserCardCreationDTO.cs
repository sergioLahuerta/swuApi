public class UserCardCreationDTO
{
    public int UserId { get; set; }
    public int CardId { get; set; }
    public int Copies { get; set; } = 1;
    public bool IsFavorite { get; set; } = false;
}