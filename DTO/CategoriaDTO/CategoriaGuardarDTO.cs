using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.CategoriaDTO;

public class CategoriaGuardarDTO
{
    [Required(ErrorMessage = "El nombre de la categoria es obligatorio")]
    [StringLength(80, ErrorMessage = "El nombre no debe superar 80 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripcion de la categoria es obligatoria")]
    [StringLength(255, ErrorMessage = "La descripcion no debe superar 255 caracteres")]
    public string Descripcion { get; set; } = string.Empty;
}
