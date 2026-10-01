using System.Text.Json.Serialization;
using MarketLocalShirts.DTO;

namespace MarketLocalShirts.DTO.CamisaDTO;

public class TallaDTO
{
    public string Talla { get; set; } = string.Empty;

    [JsonConverter(typeof(EnteroFlexibleJson))]
    public int Stock { get; set; }

    [JsonConverter(typeof(BooleanoFlexibleJson))]
    public bool Agotado { get; set; }
}
