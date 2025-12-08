using swuApi.Models;

namespace swuApi.Services
{
    public interface IPackOpeningService
    {
        public Task<List<Card>> OpenPackAsync(int packId);
    }
}