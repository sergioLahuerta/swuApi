using swuApi.Enums;

namespace swuApi.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string CardName { get; set; } = string.Empty;
        public string Subtitle { get; set; }
        public CardModelType Model { get; set; }
        public CardAspectType Aspect { get; set; }
        public CardRarityType Rarity { get; set; } = CardRarityType.Common;
        public int CardNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAcquired { get; set; }
        public bool IsPromo { get; set; } = false;
        public int CollectionId { get; set; }
        public Collection? Collection { get; set; }

        public Card()
        {
            DateAcquired = DateTime.Now;
        }
    }
}