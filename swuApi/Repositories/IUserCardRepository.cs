using swuApi.Models;

namespace swuApi.Repositories
{
    public interface IUserCardRepository : IRepository<UserCard> 
    {
        Task<UserCard?> GetByUserIdAndCardIdAsync(int userId, int cardId);
        Task UpsertCopiesAsync(int userId, int cardId, int copiesToAdd);
        Task<List<UserCard>> GetInventoryByUserIdAsync(int userId, string? sortField, string? sortDirection);
    }
}