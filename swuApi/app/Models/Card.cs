namespace swuApi.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string CardName { get; set; }
        public string Subtitle { get; set; }
        public string Model { get; set; }
        public string Aspect { get; set; }
        public int CardNumber { get; set; }
        public int Copies { get; set; }
        public int ColectionId { get; set; }
        public Colection? Colection { get; set; }
        public Card() { }
    }
}