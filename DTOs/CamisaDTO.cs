using System.Text.Json.Serialization;

namespace MarketLocalShirts.Models
{
    public class CamisaDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("precio")]
        public decimal Precio { get; set; }

        [JsonPropertyName("stock")]
        public int Stock { get; set; }

        [JsonPropertyName("agotado")]
        public bool Agotado { get; set; }

        [JsonPropertyName("imagenUrl")]
        public string? ImagenUrl { get; set; }

        [JsonPropertyName("talla")]
        public string? Talla { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("categoria")]
        public CategoriaDTO? Categoria { get; set; }

        [JsonPropertyName("marca")]
        public MarcaDTO? Marca { get; set; }

        [JsonPropertyName("tallas")]
        public List<TallaDTO>? Tallas { get; set; }

        // Propiedad de conveniencia para mostrar el nombre de la marca
        public string NombreMarca => Marca?.Nombre ?? "MLS";
    }

    public class MarcaDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }
    }

    public class CategoriaDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }
    }

    public class TallaDTO
    {
        [JsonPropertyName("talla")]
        public string Talla { get; set; } = string.Empty;

        [JsonPropertyName("stock")]
        public int Stock { get; set; }

        [JsonPropertyName("agotado")]
        public bool Agotado { get; set; }
    }
}