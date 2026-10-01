using System.Net;
using System.Net.Http.Json;
using MarketLocalShirts.DTO.PedidoDTO;

namespace MarketLocalShirts.Service;

public class PedidoService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private List<PedidoSalidaDTO>? _todos;
    private Task<List<PedidoSalidaDTO>>? _cargaTodos;
    private readonly Dictionary<int, List<PedidoSalidaDTO>> _porUsuario = new();
    private readonly Dictionary<int, Task<List<PedidoSalidaDTO>>> _cargaUsuario = new();
    private int _version;

    public PedidoService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
        _authService.OnChange += LimpiarCache;
    }

    public List<PedidoSalidaDTO>? PedidosEnMemoria => _todos;

    public bool TryPedidosDeUsuario(int usuarioId, out List<PedidoSalidaDTO> pedidos) =>
        _porUsuario.TryGetValue(usuarioId, out pedidos!);

    public Task<List<PedidoSalidaDTO>> ObtenerTodosAsync()
    {
        if (_todos != null)
            return Task.FromResult(_todos);

        return _cargaTodos ??= CargarTodos();
    }

    public Task<List<PedidoSalidaDTO>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        if (_porUsuario.TryGetValue(usuarioId, out var lista))
            return Task.FromResult(lista);

        if (_cargaUsuario.TryGetValue(usuarioId, out var enCurso))
            return enCurso;

        var tarea = CargarUsuario(usuarioId);
        _cargaUsuario[usuarioId] = tarea;
        return tarea;
    }

    private async Task<List<PedidoSalidaDTO>> CargarTodos()
    {
        var version = _version;
        try
        {
            await _authService.GetToken();
            var lista = await LeerLista("api/pedidos/lista");
            if (version == _version)
                _todos = lista;
            return lista;
        }
        finally
        {
            if (version == _version)
                _cargaTodos = null;
        }
    }

    private async Task<List<PedidoSalidaDTO>> CargarUsuario(int usuarioId)
    {
        var version = _version;
        try
        {
            await _authService.GetToken();
            var lista = await LeerLista($"api/pedidos/usuario/{usuarioId}");
            if (version == _version)
                _porUsuario[usuarioId] = lista;
            return lista;
        }
        finally
        {
            if (version == _version)
                _cargaUsuario.Remove(usuarioId);
        }
    }

    private void LimpiarCache()
    {
        _version++;
        _todos = null;
        _cargaTodos = null;
        _porUsuario.Clear();
        _cargaUsuario.Clear();
    }

    public async Task<(bool Ok, string Mensaje)> CrearAsync(PedidoGuardarDTO pedido)
    {
        LimpiarCache();
        await _authService.GetToken();
        var response = await _httpClient.PostAsJsonAsync("api/pedidos", pedido);
        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Pedido confirmado.");
    }

    public async Task<(bool Ok, string Mensaje)> CambiarEstadoAsync(int id, string estado)
    {
        LimpiarCache();
        await _authService.GetToken();
        var response = await _httpClient.PutAsJsonAsync($"api/pedidos/{id}", new PedidoEstadoDTO
        {
            Estado = estado
        });

        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Estado del pedido actualizado.");
    }

    private async Task<List<PedidoSalidaDTO>> LeerLista(string ruta)
    {
        var response = await _httpClient.GetAsync(ruta);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new List<PedidoSalidaDTO>();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ApiMensaje.Leer(response));

        return await response.Content.ReadFromJsonAsync<List<PedidoSalidaDTO>>() ?? new List<PedidoSalidaDTO>();
    }
}
