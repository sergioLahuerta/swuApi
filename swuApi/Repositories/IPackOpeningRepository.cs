using swuApi.Models;

namespace swuApi.Repositories
{
    public interface IPackOpeningRepository : IRepository<Card> 
    {
        // El método de consulta específico para la lógica del sobre.
        Task<List<Card>> GetAllCardsInCollectionAsync(int collectionId); 
    }
}