using System.Net.Http.Json;
using MarketLocalShirts.DTOs;

namespace MarketLocalShirts.Service.Pedido
{
    public class PedidoService
    {
        private readonly HttpClient _httpClient;

        public PedidoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PedidoDTO>> ObtenerMisPedidosAsync(int usuarioId)
        {
            try
            {
                var respuesta = await _httpClient.GetFromJsonAsync<List<PedidoDTO>>($"api/pedidos/usuario/{usuarioId}");
                return respuesta ?? new List<PedidoDTO>();
            }
            catch
            {
                return new List<PedidoDTO>();
            }
        }
    }
}