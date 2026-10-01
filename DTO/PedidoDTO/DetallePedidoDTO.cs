using System.Text.Json.Serialization;
using MarketLocalShirts.DTO.CamisaDTO;

namespace MarketLocalShirts.DTO.PedidoDTO;

public class DetallePedidoSalidaDTO
{
    public int Id { get; set; }
    public CamisaSalidaDTO? Camisa { get; set; }
    public string? Talla { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public string NombreProducto
    {
        get
        {
            return string.IsNullOrWhiteSpace(Camisa?.Nombre) ? "Camisa" : Camisa!.Nombre;
        }
    }

    [JsonIgnore]
    public decimal SubTotal => Subtotal;
}
