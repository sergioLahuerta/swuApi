using System;
using System.ComponentModel.DataAnnotations;

namespace swuApi.Models
{
    public class Card
    {
        public int Id { get; set; } 
        public string CardName { get; set; } = string.Empty;
        public string Subtitle { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Aspect { get; set; } 
        public int CardNumber { get; set; } 
        public int Copies { get; set; }
        public decimal Price { get; set; } 
        public DateTime DateAcquired { get; set; } 
        public bool IsPromo { get; set; } 
        public int CollectionId { get; set; }
        public Colection? Colection { get; set; }

        public Card() 
        {
            DateAcquired = DateTime.Now;
        }
    }
}