using swuApi.Models;
using swuApi.Repositories;

namespace swuApi.Services
{
    public class PackService : IService<Pack>
    {
        private readonly IRepository<Pack> _packRepository;
        
        private const int StandardShowcaseOdds = 288; // Siempre cuento con que en 1 de cada 288 sobres aprox sale una Showcase

        public PackService(IRepository<Pack> packRepository)
        {
            _packRepository = packRepository;
        }

        public async Task<List<Pack>> GetAllAsync()
        {
            return await _packRepository.GetAllAsync();
        }
        
        public async Task<List<Pack>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            return await _packRepository.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
        }

        public async Task<Pack?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.", nameof(id));

            return await _packRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Pack pack)
        {
            // Validación por nomrbe no vació
            if (string.IsNullOrWhiteSpace(pack.PackName))
                throw new ArgumentException("El nombre del sobre no puede estar vacío.", nameof(pack.PackName));

            // Validación precio no en negativo
            if (pack.Price < 0)
                throw new ArgumentException("El precio del sobre no puede ser negativo.", nameof(pack.Price));

            // Validación una Showcase en 288 sobres aprox
            if (pack.PackName.Contains("Booster Pack") && pack.ShowcaseRarityOdds != StandardShowcaseOdds)
            {
                // Regla de que los sobres estándar deben seguir la tasa de 1 en 288
                throw new ArgumentException(
                    $"Los sobres estándar (Booster Packs) deben tener una probabilidad Showcase de 1 en {StandardShowcaseOdds}.",
                    nameof(pack.ShowcaseRarityOdds)
                );
            }
            
            // 4. Validación de fecha no futura
            if (pack.ReleaseDate > DateTime.UtcNow)
                throw new ArgumentException("La fecha de lanzamiento no puede ser en el futuro.", nameof(pack.ReleaseDate));

            await _packRepository.AddAsync(pack);
        }
        
        public async Task UpdateAsync(Pack pack)
        {
            if (pack.Id <= 0)
                throw new ArgumentException("El ID no es válido para actualización.", nameof(pack.Id));
            
            if (string.IsNullOrWhiteSpace(pack.PackName))
                throw new ArgumentException("El nombre del sobre no puede estar vacío.", nameof(pack.PackName));
            
            if (pack.Price < 0)
                throw new ArgumentException("El precio del sobre no puede ser negativo.", nameof(pack.Price));

            if (pack.PackName.Contains("Booster Pack") && pack.ShowcaseRarityOdds != StandardShowcaseOdds)
            {
                throw new ArgumentException(
                    $"Los sobres estándar (Booster Packs) deben tener una probabilidad Showcase de 1 en {StandardShowcaseOdds}.",
                    nameof(pack.ShowcaseRarityOdds)
                );
            }
            
            await _packRepository.UpdateAsync(pack);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.", nameof(id));

            await _packRepository.DeleteAsync(id);
        }
    }
}