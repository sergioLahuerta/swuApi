namespace swuApi.Models {
    public class Card
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Subtitulo { get; set; }
        public string Modelo { get; set; }
        public string Aspecto { get; set; }
        public int NumeroCarta { get; set; }
        public int Cantidad { get; set; }
        public int ColeccionId { get; set; }
        public Coleccion? Coleccion { get; set; }
        public Carta() { }
    }
}