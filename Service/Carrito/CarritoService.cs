using System.Net.Http.Json;
using MarketLocalShirts.DTOs;

namespace MarketLocalShirts.Service.Carrito;

public class CarritoService
{
    private readonly HttpClient _httpClient;

    public CarritoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Obtiene la lista de ítems agregados al carrito
    public async Task<List<CarritoDTO>> ObtenerCarritoAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CarritoDTO>>("api/carrito")
                   ?? new List<CarritoDTO>();
        }
        catch
        {
            return new List<CarritoDTO>();
        }
    }

    // Actualiza la cantidad de un ítem en el carrito
    public async Task<bool> ActualizarCantidadAsync(int idItem, int nuevaCantidad)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/carrito/{idItem}", new { Cantidad = nuevaCantidad });
        return response.IsSuccessStatusCode;
    }

    // Elimina un producto específico del carrito
    public async Task<bool> EliminarDelCarritoAsync(int idItem)
    {
        var response = await _httpClient.DeleteAsync($"api/carrito/{idItem}");
        return response.IsSuccessStatusCode;
    }

    // Procesa la compra enviando la orden a la API
    public async Task<bool> ProcesarPagoAsync()
    {
        var response = await _httpClient.PostAsync("api/pedidos/checkout", null);
        return response.IsSuccessStatusCode;
    }
}