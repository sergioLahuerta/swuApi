using System.ComponentModel;
using swuApi.Enums;

namespace swuApi.DTOs
{
    public abstract class ICardDTO
    {
        public string CardName { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public CardModelType Model { get; set; }
        public CardAspectType Aspect { get; set; }
        public CardRarityType Rarity { get; set; }
        public int CardNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAcquired { get; set; }

        [DefaultValue(false)]
        public bool IsPromo { get; set; }
        public int CollectionId { get; set; }
    }
}