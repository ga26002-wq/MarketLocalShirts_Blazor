namespace MarketLocalShirts.DTO.UsuarioDTO;

public class UsuarioSalidaDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Rol { get; set; }
    public bool? Activo { get; set; }

    public string Estado => Activo == false ? "Inactiva" : "Activa";
}
