using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.UsuarioDTO;

public class UsuarioLoginDTO
{
    [Required(ErrorMessage = "El login (correo) es obligatorio")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clave es obligatoria")]
    public string Clave { get; set; } = string.Empty;
}
