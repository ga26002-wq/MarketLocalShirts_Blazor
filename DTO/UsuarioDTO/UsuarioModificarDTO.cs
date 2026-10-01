using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.UsuarioDTO;

public class UsuarioModificarDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no debe superar 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es valido")]
    public string Correo { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El telefono no debe superar 20 caracteres")]
    public string? Telefono { get; set; }

    [ContrasenaOpcional]
    public string? Contrasena { get; set; }

    public string? Rol { get; set; }
    public bool? Activo { get; set; }
}
