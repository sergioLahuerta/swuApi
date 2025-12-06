using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace swuApi.Models
{
    public class Colection
    {
        public int Id { get; set; } 
        public string CollectionName { get; set; } = string.Empty;
        public string Color { get; set; }
        public int NumCards { get; set; } 
        public decimal EstimatedValue { get; set; } 
        public DateTime CreationDate { get; set; } 
        public bool IsComplete { get; set; } 
        public ICollection<Card> Cards { get; set; }

        public Colection()
        {
            Cards = new List<Card>();
            CreationDate = DateTime.Now;
        }
    }
}