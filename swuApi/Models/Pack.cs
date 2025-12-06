using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace swuApi.Models
{
    public class Pack
    {
        public int Id { get; set; } 
        public string PackName { get; set; } = string.Empty;
        public int NumberOfCards { get; set; } = 16;
        public int ShowcaseRarityOdds { get; set; }
        public bool GuaranteesRare { get; set; } = true;
        public decimal Price { get; set; } = 4.99M;
        public DateTime ReleaseDate { get; set; }
        public int CollectionId { get; set; }
        public Collection? Collection { get; set; }

        public Pack()
        {
            ReleaseDate = DateTime.UtcNow;
        }
    }
}