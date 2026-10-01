using System.Text.Json.Serialization;
using MarketLocalShirts.DTO;
using MarketLocalShirts.DTO.CategoriaDTO;
using MarketLocalShirts.DTO.MarcaDTO;

namespace MarketLocalShirts.DTO.CamisaDTO;

public class CamisaSalidaDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }

    [JsonConverter(typeof(EnteroFlexibleJson))]
    public int Stock { get; set; }

    [JsonConverter(typeof(BooleanoFlexibleJson))]
    public bool Agotado { get; set; }
    public string? ImagenUrl { get; set; }
    public string? Talla { get; set; }
    public string? Color { get; set; }
    public List<TallaDTO>? Tallas { get; set; }
    public CategoriaSalidaDTO? Categoria { get; set; }
    public MarcaSalidaDTO? Marca { get; set; }

    public string NombreMarca => Marca?.Nombre ?? "MLS";
}
