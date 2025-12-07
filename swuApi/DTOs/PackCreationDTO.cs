namespace swuApi.DTOs
{
    public class PackCreationDTO
    {
        public string PackName { get; set; } = string.Empty;
        public int NumberOfCards { get; set; } = 16;
        public int ShowcaseRarityOdds { get; set; }
        public bool GuaranteesRare { get; set; } = true;
        public decimal Price { get; set; } = 4.99M;
        public DateTime ReleaseDate { get; set; }
        public int CollectionId { get; set; }
    }
}