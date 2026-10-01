using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketLocalShirts.DTO;

public class EnteroFlexibleJson : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return 0;

        if (reader.TokenType == JsonTokenType.String)
            return int.TryParse(reader.GetString(), out var texto) ? texto : 0;

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out var entero))
                return entero;
            if (reader.TryGetDecimal(out var dec))
                return (int)dec;
        }

        return 0;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value);
}

public class BooleanoFlexibleJson : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.String => bool.TryParse(reader.GetString(), out var valor) && valor,
            _ => false
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteBooleanValue(value);
}
