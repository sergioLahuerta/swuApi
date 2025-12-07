using System.ComponentModel;

namespace swuApi.DTOs
{
    public class CollectionCreationDTO
    {
        public string CollectionName { get; set; } = string.Empty;
        public string Color { get; set; }
        public int NumCards { get; set; }
        public decimal EstimatedValue { get; set; }
        public DateTime? CreationDate { get; set; }

        [DefaultValue(false)]
        public bool IsComplete { get; set; }
    }
}