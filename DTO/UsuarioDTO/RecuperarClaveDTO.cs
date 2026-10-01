using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.UsuarioDTO;

public class RecuperarClaveDTO
{
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es valido")]
    public string Correo { get; set; } = string.Empty;
}
