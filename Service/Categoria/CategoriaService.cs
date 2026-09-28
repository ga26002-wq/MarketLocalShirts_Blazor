using MarketLocalShirts.DTOs;
using MarketLocalShirts.Models;
using System.Net.Http.Json;

namespace MarketLocalShirts.Service.Categoria
{
    public class CategoriaService
    {
        private readonly HttpClient _httpClient;

        public CategoriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoriaDTO>> ObtenerCategoriasAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CategoriaDTO>>("api/categorias")
                       ?? new List<CategoriaDTO>();
            }
            catch
            {
                return new List<CategoriaDTO>();
            }
        }
    }
}