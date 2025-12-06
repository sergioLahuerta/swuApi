using System;
using System.ComponentModel;

namespace swuApi.DTOs
{
    public class CardCreationDTO
    {
        public string CardName { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Model { get; set; } = string.Empty;
        public string? Aspect { get; set; }
        public int CardNumber { get; set; }
        public int Copies { get; set; } = 0;
        public decimal Price { get; set; }
        public DateTime DateAcquired { get; set; } = default;
        
        [DefaultValue(false)]
        public bool IsPromo { get; set; }
        public int CollectionId { get; set; }
    }
}