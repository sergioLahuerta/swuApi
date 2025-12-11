using swuApi.Enums;

namespace swuApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string MessageReview { get; set; }
        public ReviewValueType Stars { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}