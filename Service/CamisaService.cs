using System.Net;
using System.Net.Http.Json;
using MarketLocalShirts.DTO.CamisaDTO;

namespace MarketLocalShirts.Service;

public class CamisaService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private List<CamisaSalidaDTO>? _cache;
    private Task<List<CamisaSalidaDTO>>? _carga;

    public CamisaService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public Task<List<CamisaSalidaDTO>> ObtenerTodasAsync()
    {
        if (_cache != null)
            return Task.FromResult(_cache);

        return _carga ??= CargarLista();
    }

    private async Task<List<CamisaSalidaDTO>> CargarLista()
    {
        try
        {
            var lista = await LeerLista("api/camisas/lista");
            _cache = lista;
            return lista;
        }
        finally
        {
            _carga = null;
        }
    }

    public async Task<CamisaSalidaDTO?> ObtenerPorIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/camisas/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CamisaSalidaDTO>();
    }

    public async Task<(bool Ok, string Mensaje, CamisaSalidaDTO? Producto)> GuardarAsync(CamisaGuardarDTO camisa, int? id)
    {
        _cache = null;
        await _authService.GetToken();
        var response = id is null or 0
            ? await _httpClient.PostAsJsonAsync("api/camisas", camisa)
            : await _httpClient.PutAsJsonAsync($"api/camisas/{id}", camisa);

        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response), null);

        var producto = await response.Content.ReadFromJsonAsync<CamisaSalidaDTO>();
        return (true, "Producto guardado.", producto);
    }

    public async Task<(bool Ok, string Mensaje)> EliminarAsync(int id)
    {
        _cache = null;
        await _authService.GetToken();
        var response = await _httpClient.DeleteAsync($"api/camisas/{id}");
        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Producto eliminado.");
    }

    private async Task<List<CamisaSalidaDTO>> LeerLista(string ruta)
    {
        var response = await _httpClient.GetAsync(ruta);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new List<CamisaSalidaDTO>();

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<CamisaSalidaDTO>>() ?? new List<CamisaSalidaDTO>();
    }
}
