using swuApi.Enums;

namespace swuApi.ReviewDTOs
{
    public abstract class IReviewDTO
    {
        public DateTime CreationDate { get; set; }
        public string MessageReview { get; set; }
        public ReviewValueType Stars { get; set; }
        public int UserId { get; set; }
    }
}