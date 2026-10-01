using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.UsuarioDTO;

public class RestablecerClaveDTO
{
    [Required(ErrorMessage = "El token de recuperacion es obligatorio")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contrasena es obligatoria")]
    [MinLength(6, ErrorMessage = "La nueva contrasena debe tener al menos 6 caracteres")]
    public string NuevaClave { get; set; } = string.Empty;
}
