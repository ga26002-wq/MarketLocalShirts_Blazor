using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MarketLocalShirts.DTOs;

namespace MarketLocalShirts.Service.Auth
{
    public class AuthService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;

        private string? _token;

        public event Action? OnChange;

        public AuthService(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> SetToken(string token)
        {
            _token = token;
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            }
            catch { }

            AjustarCabeceraAuth(token);
            NotifyStateChanged();
            return _token!;
        }

        public async Task Logout()
        {
            _token = null;
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            }
            catch { }

            AjustarCabeceraAuth(null);
            NotifyStateChanged();
        }

        public async Task<string?> GetToken()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                AjustarCabeceraAuth(_token);
                return _token;
            }

            try
            {
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _token = token;
                    AjustarCabeceraAuth(_token);
                    return _token;
                }
            }
            catch { }

            return null;
        }

        private void AjustarCabeceraAuth(string? token)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }
            catch { }
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await GetToken();
            if (string.IsNullOrEmpty(token)) return false;
            return !IsTokenExpired(token);
        }

        public bool IsTokenExpired(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return true;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token)) return true;

                var jwt = handler.ReadJwtToken(token);
                var exp = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                if (string.IsNullOrEmpty(exp)) return true;

                if (long.TryParse(exp, out var seconds))
                {
                    var expiry = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
                    return expiry <= DateTime.UtcNow;
                }

                return true;
            }
            catch
            {
                return true;
            }
        }

        // =======================================================
        //  MÉTODOS CONECTADOS A TU API DE SPRING BOOT (SWAGGER)
        // =======================================================

        // GET: api/usuarios/perfil
        public async Task<PerfilUsuarioDTO?> ObtenerPerfilRealAsync()
        {
            try
            {
                await GetToken(); // Asegura la cabecera Bearer Token en HttpClient
                return await _httpClient.GetFromJsonAsync<PerfilUsuarioDTO>("api/usuarios/perfil");
            }
            catch
            {
                return null;
            }
        }

        // PUT: api/usuarios/perfil  o  PUT: api/usuarios/{id}
        public async Task<bool> ActualizarPerfilRealAsync(PerfilUsuarioDTO perfil)
        {
            try
            {
                await GetToken(); // Asegura la cabecera Bearer Token

                // Petición PUT a la API enviando el objeto de perfil
                var response = await _httpClient.PutAsJsonAsync($"api/usuarios/{perfil.Id}", perfil);

                if (!response.IsSuccessStatusCode)
                {
                    // Intento alternativo en caso de que tu controller use api/usuarios/perfil
                    response = await _httpClient.PutAsJsonAsync("api/usuarios/perfil", perfil);
                }

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}