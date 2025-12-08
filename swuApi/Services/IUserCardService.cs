using swuApi.Models;

namespace swuApi.Services
{
    public interface IUserCardService 
    {
        Task<UserCard> AddCardToInventoryAsync(UserCardCreationDTO dto);
        Task RemoveCardFromInventoryAsync(int userId, int cardId, int copiesToRemove);
        Task<List<UserCard>> GetInventoryByUserIdAsync(int userId, string? sortField, string? sortDirection);
        Task UpdateCardStatusAsync(int userId, int cardId, bool isFavorite);
    }
}