using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.PedidoDTO;

public class PedidoGuardarDTO
{
    [MinLength(1, ErrorMessage = "El pedido debe incluir al menos una camisa")]
    public List<DetallePedidoGuardarDTO> Detalles { get; set; } = new();
}

public class DetallePedidoGuardarDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "La camisa es obligatoria")]
    public int CamisaId { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria")]
    [StringLength(10, ErrorMessage = "La talla no debe superar 10 caracteres")]
    public string Talla { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int Cantidad { get; set; }
}

public class PedidoEstadoDTO
{
    [Required(ErrorMessage = "El estado del pedido es obligatorio")]
    public string Estado { get; set; } = string.Empty;
}
