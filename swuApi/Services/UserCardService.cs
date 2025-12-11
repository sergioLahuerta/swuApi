using swuApi.DTOs;
using swuApi.Models;
using swuApi.Repositories;

namespace swuApi.Services
{
    public class UserCardService : IUserCardService
    {
        private readonly IUserCardRepository _userCardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPackOpeningRepository _cardRepository;

        public UserCardService(
            IUserCardRepository userCardRepository,
            IUserRepository userRepository,
            IPackOpeningRepository cardRepository)
        {
            _userCardRepository = userCardRepository;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
        }

        // Métodos específicos de UserCardService (como los que tienes en tu servicio)
        public async Task<UserCard> AddCardToInventoryAsync(UserCardCreationDTO dto)
        {
            if (dto.UserId <= 0 || dto.CardId <= 0 || dto.Copies <= 0)
                throw new ArgumentException("Los IDs y la cantidad deben ser positivos.");

            var userExists = await _userRepository.GetByIdAsync(dto.UserId);
            if (userExists == null)
                throw new KeyNotFoundException($"Usuario con ID {dto.UserId} no encontrado.");

            var cardExists = await _cardRepository.GetByIdAsync(dto.CardId);
            if (cardExists == null)
                throw new KeyNotFoundException($"Carta con ID {dto.CardId} no encontrada.");

            await _userCardRepository.UpsertCopiesAsync(dto.UserId, dto.CardId, dto.Copies);

            var updatedEntry = await _userCardRepository.GetByUserIdAndCardIdAsync(dto.UserId, dto.CardId);
            return updatedEntry!;
        }

        public async Task<List<UserCard>> GetInventoryByUserIdAsync(int userId, string? sortField, string? sortDirection)
        {
            if (userId <= 0)
                throw new ArgumentException("El ID de usuario no es válido.", nameof(userId));

            var userExists = await _userRepository.GetByIdAsync(userId);
            if (userExists == null)
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado.");

            return await _userCardRepository.GetInventoryByUserIdAsync(userId, sortField, sortDirection);
        }

        public async Task RemoveCardFromInventoryAsync(int userId, int cardId, int copiesToRemove)
        {
            if (userId <= 0 || cardId <= 0 || copiesToRemove <= 0)
                throw new ArgumentException("Los IDs y la cantidad a remover deben ser positivos.");

            var existingEntry = await _userCardRepository.GetByUserIdAndCardIdAsync(userId, cardId);

            if (existingEntry == null)
                throw new KeyNotFoundException("Entrada de inventario no encontrada para el usuario y la carta especificados.");

            if (copiesToRemove > existingEntry.Copies)
                throw new ArgumentException($"No se pueden remover {copiesToRemove} copias. El usuario solo tiene {existingEntry.Copies}.", nameof(copiesToRemove));

            if (copiesToRemove == existingEntry.Copies)
            {
                await _userCardRepository.DeleteAsync(existingEntry.Id);
            }
            else
            {
                existingEntry.Copies -= copiesToRemove;
                await _userCardRepository.UpdateAsync(existingEntry);
            }
        }

        public async Task UpdateCardStatusAsync(int userId, int cardId, bool isFavorite)
        {
            if (userId <= 0 || cardId <= 0)
                throw new ArgumentException("IDs no válidos.");

            var existingEntry = await _userCardRepository.GetByUserIdAndCardIdAsync(userId, cardId);

            if (existingEntry == null)
                throw new KeyNotFoundException("Entrada de inventario no encontrada.");

            existingEntry.IsFavorite = isFavorite;
            await _userCardRepository.UpdateAsync(existingEntry);
        }

        // Métodos de la interfaz IService<UserCard> que debes implementar

        public async Task<UserCard> GetByIdAsync(int id)
        {
            return await _userCardRepository.GetByIdAsync(id);
        }

        public async Task<List<UserCard>> GetAllAsync()
        {
            return await _userCardRepository.GetAllAsync();
        }

        public async Task AddAsync(UserCard entity)
        {
            await _userCardRepository.AddAsync(entity);
        }

        public async Task UpdateAsync(UserCard entity)
        {
            await _userCardRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _userCardRepository.DeleteAsync(id);
        }

        public async Task<List<UserCard>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            return await _userCardRepository.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
        }
    }
}