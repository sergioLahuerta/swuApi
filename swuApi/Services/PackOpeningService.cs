using swuApi.Models;
using swuApi.Repositories;
using swuApi.Enums;

namespace swuApi.Services
{
    public class PackOpeningService : IPackOpeningService
    {
        private readonly IRepository<Pack> _packRepository;
        private readonly IPackOpeningRepository _cardRepository;

        public PackOpeningService(IRepository<Pack> packRepository, IPackOpeningRepository cardRepository)
        {
            _packRepository = packRepository;
            _cardRepository = cardRepository;
        }

        public async Task<List<Card>> OpenPackAsync(int packId)
        {
            // Obtener el Pack
            var pack = await _packRepository.GetByIdAsync(packId);
            if (pack == null) throw new KeyNotFoundException($"Sobre con ID {packId} no encontrado.");

            var generatedCards = new List<Card>();
            
            var availableCards = await _cardRepository.GetAllCardsInCollectionAsync(pack.CollectionId);
            
            // Carta Rara Asegurada
            generatedCards.Add(SelectRandomCard(availableCards, CardRarityType.Rare));

            // 9x Common
            for (int i = 0; i < 9; i++)
                generatedCards.Add(SelectRandomCard(availableCards, CardRarityType.Common));

            // 4x Uncommon
            for (int i = 0; i < 4; i++)
                generatedCards.Add(SelectRandomCard(availableCards, CardRarityType.Uncommon));
                
            // Legendaria en 1 de cada 5 sobres (Random.Next(1, 5) == 1)
            if (Random.Shared.Next(1, 5) == 1)
            {
                generatedCards.Add(SelectRandomCard(availableCards, CardRarityType.Legendary));
            } else {
                // Si no sale Legendaria, añade otra carta común
                generatedCards.Add(SelectRandomCard(availableCards, CardRarityType.Common));
            }

            return generatedCards;
        }
        
        private Card SelectRandomCard(List<Card> allCards, CardRarityType rarity)
        {
            var pool = allCards.Where(c => c.Rarity == rarity).ToList();

            if (!pool.Any())
            {
                // Si no hay cartas de esa rareza, devolvemos una carta común para evitar errores.
                return allCards.First(c => c.Rarity == CardRarityType.Common);
            }

            int index = Random.Shared.Next(pool.Count);
            return pool[index];
        }
    }
}