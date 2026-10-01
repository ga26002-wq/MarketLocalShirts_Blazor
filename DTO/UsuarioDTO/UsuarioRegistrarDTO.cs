using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.UsuarioDTO;

public class UsuarioRegistrarDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no debe superar 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El telefono no debe superar 20 caracteres")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El login (correo) es obligatorio")]
    [EmailAddress(ErrorMessage = "El login debe ser un correo valido")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clave es obligatoria")]
    [MinLength(6, ErrorMessage = "La clave debe tener al menos 6 caracteres")]
    public string Clave { get; set; } = string.Empty;
}
