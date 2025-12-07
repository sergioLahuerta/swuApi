using System.ComponentModel;
using swuApi.Enums;

namespace swuApi.DTOs
{
    public class CardCreationDTO
    {
        public string CardName { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public CardModelType Model { get; set; }
        public CardAspectType Aspect { get; set; }
        public CardRarityType Rarity { get; set; } = CardRarityType.Common;
        public int CardNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAcquired { get; set; } = default;
        
        [DefaultValue(false)]
        public bool IsPromo { get; set; }
        public int CollectionId { get; set; }
    }
}