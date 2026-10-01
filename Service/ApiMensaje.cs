using System.Net.Http.Json;
using System.Text.Json;

namespace MarketLocalShirts.Service;

public static class ApiMensaje
{
    public static async Task<string> Leer(HttpResponseMessage response)
    {
        var cuerpo = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(cuerpo))
            return $"Error {(int)response.StatusCode}";

        try
        {
            using var documento = JsonDocument.Parse(cuerpo);
            if (documento.RootElement.TryGetProperty("mensaje", out var mensaje))
            {
                var texto = mensaje.GetString();
                if (!string.IsNullOrWhiteSpace(texto))
                    return texto;
            }
        }
        catch (JsonException)
        {
        }

        return cuerpo;
    }

    public static async Task<T?> LeerJson<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
