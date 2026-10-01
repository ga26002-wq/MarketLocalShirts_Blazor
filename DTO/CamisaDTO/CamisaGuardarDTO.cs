using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO.CamisaDTO;

public class CamisaGuardarDTO
{
    [Required(ErrorMessage = "El nombre de la camisa es obligatorio")]
    [StringLength(150, ErrorMessage = "El nombre no debe superar 150 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripcion es obligatoria")]
    [StringLength(500, ErrorMessage = "La descripcion no debe superar 500 caracteres")]
    public string? Descripcion { get; set; }

    [PrecioMayorCero]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La URL o ruta de la imagen es obligatoria")]
    [StringLength(500, ErrorMessage = "La URL o ruta de la imagen no debe superar 500 caracteres")]
    public string? ImagenUrl { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria")]
    [StringLength(10, ErrorMessage = "La talla no debe superar 10 caracteres")]
    public string Talla { get; set; } = string.Empty;

    [Required(ErrorMessage = "El color es obligatorio")]
    [StringLength(40, ErrorMessage = "El color no debe superar 40 caracteres")]
    public string? Color { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La categoria es obligatoria")]
    public int CategoriaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La marca es obligatoria")]
    public int MarcaId { get; set; }
}
