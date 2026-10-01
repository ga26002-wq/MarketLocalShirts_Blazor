using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.UsuarioDTO;

public class PerfilUsuarioDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no debe superar 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Debes escribir un correo válido, por ejemplo nombre@correo.com")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El telefono es obligatorio")]
    [StringLength(20, ErrorMessage = "El telefono no debe superar 20 caracteres")]
    public string Telefono { get; set; } = string.Empty;
}
