using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MarketLocalShirts.DTO.UsuarioDTO;

namespace MarketLocalShirts.Service;

public class UsuarioService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public UsuarioService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<UsuarioSalidaDTO>> ObtenerTodosAsync()
    {
        await _authService.GetToken();
        var response = await _httpClient.GetAsync("api/usuarios/lista");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new List<UsuarioSalidaDTO>();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ApiMensaje.Leer(response));

        return await response.Content.ReadFromJsonAsync<List<UsuarioSalidaDTO>>() ?? new List<UsuarioSalidaDTO>();
    }

    public async Task<(bool Ok, string Mensaje)> ActualizarAsync(int id, UsuarioModificarDTO usuario)
    {
        await _authService.GetToken();
        var opciones = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var response = await _httpClient.PutAsJsonAsync($"api/usuarios/{id}", usuario, opciones);
        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Usuario actualizado.");
    }
}
