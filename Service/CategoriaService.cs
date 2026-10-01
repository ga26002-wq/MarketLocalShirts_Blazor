using System.Net;
using System.Net.Http.Json;
using MarketLocalShirts.DTO.CategoriaDTO;

namespace MarketLocalShirts.Service;

public class CategoriaService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private List<CategoriaSalidaDTO>? _cache;
    private Task<List<CategoriaSalidaDTO>>? _carga;

    public CategoriaService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public Task<List<CategoriaSalidaDTO>> ObtenerTodasAsync()
    {
        if (_cache != null)
            return Task.FromResult(_cache);

        return _carga ??= CargarLista();
    }

    private async Task<List<CategoriaSalidaDTO>> CargarLista()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/categorias/lista");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _cache = new List<CategoriaSalidaDTO>();
                return _cache;
            }

            response.EnsureSuccessStatusCode();
            _cache = await response.Content.ReadFromJsonAsync<List<CategoriaSalidaDTO>>() ?? new List<CategoriaSalidaDTO>();
            return _cache;
        }
        finally
        {
            _carga = null;
        }
    }

    public async Task<CategoriaSalidaDTO?> ObtenerPorIdAsync(int id)
    {
        var categorias = await ObtenerTodasAsync();
        return categorias.FirstOrDefault(categoria => categoria.Id == id);
    }

    public async Task<(bool Ok, string Mensaje)> GuardarAsync(CategoriaGuardarDTO categoria, int? id)
    {
        _cache = null;
        await _authService.GetToken();
        var response = id is null or 0
            ? await _httpClient.PostAsJsonAsync("api/categorias", categoria)
            : await _httpClient.PutAsJsonAsync($"api/categorias/{id}", categoria);

        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Categoría guardada.");
    }

    public async Task<(bool Ok, string Mensaje)> EliminarAsync(int id)
    {
        _cache = null;
        await _authService.GetToken();
        var response = await _httpClient.DeleteAsync($"api/categorias/{id}");
        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Categoría eliminada.");
    }
}
