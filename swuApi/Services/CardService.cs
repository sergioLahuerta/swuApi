using swuApi.Models;
using swuApi.Repositories;

namespace swuApi.Services
{
    public class CardService : IService<Card>
    {
        private readonly IRepository<Card> _cardRepository;

        public CardService(IRepository<Card> cardRepository)
        {
            _cardRepository = cardRepository;
            
        }

        public async Task<List<Card>> GetAllAsync()
        {
            return await _cardRepository.GetAllAsync();
        }

        public async Task<Card?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");

            return await _cardRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Card card)
        {
            if (string.IsNullOrWhiteSpace(card.CardName))
                throw new ArgumentException("El nombre del card no puede estar vacío.");

            await _cardRepository.AddAsync(card);
        }

        public async Task UpdateAsync(Card card)
        {
            if (card.Id <= 0)
                throw new ArgumentException("El ID no es válido para actualización.");

            if (string.IsNullOrWhiteSpace(card.CardName))
                throw new ArgumentException("El nombre del card no puede estar vacío.");

            await _cardRepository.UpdateAsync(card);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.");

            await _cardRepository.DeleteAsync(id);
        }
    }
}