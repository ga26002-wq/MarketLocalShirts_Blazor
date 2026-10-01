using MarketLocalShirts.DTO.CamisaDTO;
using MarketLocalShirts.DTO.UsuarioDTO;

namespace MarketLocalShirts.DTO.PedidoDTO;

public class PedidoSalidaDTO
{
    public int Id { get; set; }
    public UsuarioSalidaDTO? Usuario { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public List<DetallePedidoSalidaDTO> Detalles { get; set; } = new();

    public string FechaFormatted => Fecha.ToString("dd/MM/yyyy HH:mm");

    public string EstadoTexto
    {
        get
        {
            var valor = (Estado ?? string.Empty).Trim();
            return valor.ToUpperInvariant() switch
            {
                "CONFIRMADO" => "Completado",
                "PENDIENTE" => "Pendiente",
                "CANCELADO" => "Cancelado",
                "COMPLETADO" => "Completado",
                "" => "Completado",
                _ => valor
            };
        }
    }
}
