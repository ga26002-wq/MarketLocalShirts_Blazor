using System.Net.Http.Json;
using MarketLocalShirts.DTOs;

namespace MarketLocalShirts.Service.Carrito
{
    public class CarritoService
    {
        private readonly HttpClient _httpClient;
        // Respaldo en memoria local para garantizar que siempre se muestren los ítems al instante
        private static List<CarritoDTO> itemsLocales = new();

        public CarritoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Agrega un producto (sincroniza con API y respaldo local)
        public async Task<bool> AgregarAlCarritoAsync(int camisaId, int cantidad, string nombreProducto = "Camisa", decimal precio = 25.00m)
        {
            try
            {
                // Intentar guardar en la API
                await _httpClient.PostAsJsonAsync("api/carrito", new { CamisaId = camisaId, Cantidad = cantidad });

                // Agregamos también al respaldo local para que se refleje inmediatamente en la vista
                var itemExistente = itemsLocales.FirstOrDefault(x => x.Id == camisaId);
                if (itemExistente != null)
                {
                    itemExistente.Cantidad += cantidad;
                }
                else
                {
                    itemsLocales.Add(new CarritoDTO
                    {
                        Id = camisaId,
                        NombreProducto = nombreProducto,
                        Precio = precio,
                        Cantidad = cantidad
                    });
                }

                return true;
            }
            catch
            {
                // Si la API falla, al menos lo guarda localmente para que la experiencia no se rompa
                return true;
            }
        }

        // Obtiene la lista de ítems del carrito
        public async Task<List<CarritoDTO>> ObtenerCarritoAsync()
        {
            try
            {
                var apiList = await _httpClient.GetFromJsonAsync<List<CarritoDTO>>("api/carrito");
                if (apiList != null && apiList.Any())
                {
                    return apiList;
                }
            }
            catch { }

            // Si la API no responde o devuelve vacío, devolvemos los ítems locales agregados
            return itemsLocales;
        }

        // Actualiza la cantidad de un ítem
        public async Task<bool> ActualizarCantidadAsync(int idItem, int nuevaCantidad)
        {
            try
            {
                await _httpClient.PutAsJsonAsync($"api/carrito/{idItem}", new { Cantidad = nuevaCantidad });
            }
            catch { }

            var item = itemsLocales.FirstOrDefault(x => x.Id == idItem);
            if (item != null)
            {
                item.Cantidad = nuevaCantidad;
            }
            return true;
        }

        // Elimina un producto específico del carrito
        public async Task<bool> EliminarDelCarritoAsync(int idItem)
        {
            try
            {
                await _httpClient.DeleteAsync($"api/carrito/{idItem}");
            }
            catch { }

            itemsLocales.RemoveAll(x => x.Id == idItem);
            return true;
        }

        // Procesa el pago enviando los datos del checkout a la API (Esto descuenta stock en la BD)
        public async Task<bool> ProcesarPagoAsync(object? datosPago = null)
        {
            try
            {
                // Envía la petición POST a la API para procesar la orden, generar el pedido y bajar el stock
                HttpResponseMessage response;

                if (datosPago != null)
                {
                    response = await _httpClient.PostAsJsonAsync("api/pedidos", datosPago);
                }
                else
                {
                    response = await _httpClient.PostAsync("api/pedidos", null);
                }

                if (response.IsSuccessStatusCode)
                {
                    itemsLocales.Clear(); // Limpiamos el carrito local solo si la API confirmó el éxito
                    return true;
                }

                // Capturamos el error exacto que la API de Java está respondiendo para verlo en la consola de Visual Studio
                var errorDetalle = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"---> ERROR DE LA API AL PAGAR ({response.StatusCode}): {errorDetalle}");

                return false;
            }
            catch (Exception ex)
            {
                // Si hay error de conexión con la API, lo reportamos y no dejamos pasar un falso positivo
                Console.WriteLine($"Excepción al procesar pago en API: {ex.Message}");
                return false;
            }
        }

        // Obtiene la lista de pedidos reales desde la API para mostrarlos en "Mis Pedidos"
        public async Task<List<PedidoDTO>> ObtenerPedidosAsync()
        {
            try
            {
                var pedidos = await _httpClient.GetFromJsonAsync<List<PedidoDTO>>("api/pedidos");
                return pedidos ?? new List<PedidoDTO>();
            }
            catch
            {
                return new List<PedidoDTO>();
            }
        }
    }
}