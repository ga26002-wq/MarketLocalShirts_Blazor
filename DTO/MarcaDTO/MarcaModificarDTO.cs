using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.MarcaDTO;

public class MarcaModificarDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la marca es obligatorio")]
    [StringLength(80, ErrorMessage = "El nombre no debe superar 80 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripcion de la marca es obligatoria")]
    [StringLength(255, ErrorMessage = "La descripcion no debe superar 255 caracteres")]
    public string Descripcion { get; set; } = string.Empty;
}
