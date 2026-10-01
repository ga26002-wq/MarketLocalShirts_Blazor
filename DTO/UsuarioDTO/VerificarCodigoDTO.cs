using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.UsuarioDTO;

public class VerificarCodigoDTO
{
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es valido")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código es obligatorio")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe tener 6 dígitos")]
    public string Codigo { get; set; } = string.Empty;
}
