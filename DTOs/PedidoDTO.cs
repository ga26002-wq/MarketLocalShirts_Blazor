namespace MarketLocalShirts.DTOs;

public class PedidoDTO
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<DetallePedidoDTO> Detalles { get; set; } = new();
}

public class DetallePedidoDTO
{
    public int Id { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal SubTotal { get; set; }
}