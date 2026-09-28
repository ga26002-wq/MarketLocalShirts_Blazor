namespace MarketLocalShirts.DTOs
{
    public class CarritoDTO
    {
        public int Id { get; set; }
        public int CamisaId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal SubTotal => Precio * Cantidad;
    }
}