using System.Net;
using System.Net.Http.Json;
using MarketLocalShirts.DTO.MarcaDTO;

namespace MarketLocalShirts.Service;

public class MarcaService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private List<MarcaSalidaDTO>? _cache;
    private Task<List<MarcaSalidaDTO>>? _carga;

    public MarcaService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public Task<List<MarcaSalidaDTO>> ObtenerTodasAsync()
    {
        if (_cache != null)
            return Task.FromResult(_cache);

        return _carga ??= CargarLista();
    }

    private async Task<List<MarcaSalidaDTO>> CargarLista()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/marcas/lista");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _cache = new List<MarcaSalidaDTO>();
                return _cache;
            }

            response.EnsureSuccessStatusCode();
            _cache = await response.Content.ReadFromJsonAsync<List<MarcaSalidaDTO>>() ?? new List<MarcaSalidaDTO>();
            return _cache;
        }
        finally
        {
            _carga = null;
        }
    }

    public async Task<MarcaSalidaDTO?> ObtenerPorIdAsync(int id)
    {
        var marcas = await ObtenerTodasAsync();
        return marcas.FirstOrDefault(marca => marca.Id == id);
    }

    public async Task<(bool Ok, string Mensaje)> GuardarAsync(MarcaGuardarDTO marca, int? id)
    {
        _cache = null;
        await _authService.GetToken();
        var response = id is null or 0
            ? await _httpClient.PostAsJsonAsync("api/marcas", marca)
            : await _httpClient.PutAsJsonAsync($"api/marcas/{id}", marca);

        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Marca guardada.");
    }

    public async Task<(bool Ok, string Mensaje)> EliminarAsync(int id)
    {
        _cache = null;
        await _authService.GetToken();
        var response = await _httpClient.DeleteAsync($"api/marcas/{id}");
        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Marca eliminada.");
    }
}
