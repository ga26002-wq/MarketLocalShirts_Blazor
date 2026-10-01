using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MarketLocalShirts.DTO.UsuarioDTO;
using Microsoft.JSInterop;

namespace MarketLocalShirts.Service;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private string? _token;
    private bool _tokenLeido;
    private Task<string?>? _lecturaToken;
    private PerfilUsuarioDTO? _perfil;
    private Task<PerfilUsuarioDTO?>? _cargaPerfil;

    public event Action? OnChange;

    public AuthService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    public async Task<string> SetToken(string token)
    {
        _token = token;
        _tokenLeido = true;
        _perfil = null;
        _cargaPerfil = null;
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        }
        catch
        {
        }

        AjustarCabeceraAuth(token);
        OnChange?.Invoke();
        return _token;
    }

    public async Task Logout()
    {
        _token = null;
        _tokenLeido = true;
        _perfil = null;
        _cargaPerfil = null;
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userEmail");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
        }
        catch
        {
        }

        AjustarCabeceraAuth(null);
        OnChange?.Invoke();
    }

    public Task<string?> GetToken()
    {
        if (_tokenLeido)
            return Task.FromResult(TokenListo());

        return _lecturaToken ??= LeerTokenGuardado();
    }

    private async Task<string?> LeerTokenGuardado()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrWhiteSpace(token))
                token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                token = token.Trim().Trim('"');
                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token[7..].Trim();

                if (!IsTokenExpired(token))
                    _token = token;
            }

            _tokenLeido = true;
        }
        catch
        {
            _tokenLeido = false;
        }
        finally
        {
            _lecturaToken = null;
        }

        return TokenListo();
    }

    private string? TokenListo()
    {
        if (string.IsNullOrEmpty(_token))
            return null;

        if (IsTokenExpired(_token))
        {
            _token = null;
            AjustarCabeceraAuth(null);
            return null;
        }

        AjustarCabeceraAuth(_token);
        return _token;
    }

    public async Task<bool> IsAuthenticated()
    {
        var token = await GetToken();
        return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
    }

    public bool IsTokenExpired(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return true;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return true;

            var jwt = handler.ReadJwtToken(token);
            var exp = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (!long.TryParse(exp, out var seconds))
                return true;

            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime <= DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }

    public bool TokenEsAdmin(string? token)
    {
        if (string.IsNullOrWhiteSpace(token) || IsTokenExpired(token))
            return false;

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            if (jwt.Claims.Any(c =>
                    (c.Type is "roles" or "role" or "authorities") &&
                    c.Value.Contains("ADMIN", StringComparison.OrdinalIgnoreCase)))
                return true;

            if (jwt.Payload.TryGetValue("roles", out var roles) &&
                roles?.ToString()?.Contains("ADMIN", StringComparison.OrdinalIgnoreCase) == true)
                return true;
        }
        catch
        {
        }

        return false;
    }

    public async Task<(bool Ok, string? Token, string Mensaje)> LoginAsync(string login, string clave)
    {
        AjustarCabeceraAuth(null);
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new UsuarioLoginDTO
        {
            Login = login,
            Clave = clave
        });

        if (!response.IsSuccessStatusCode)
        {
            var texto = await ApiMensaje.Leer(response);
            if (texto.Contains("disabled", StringComparison.OrdinalIgnoreCase)
                || texto.Contains("bloque", StringComparison.OrdinalIgnoreCase)
                || texto.Contains("suspend", StringComparison.OrdinalIgnoreCase))
                return (false, null, "Esta cuenta está suspendida. No puede iniciar sesión.");

            if (texto.StartsWith("Error ", StringComparison.Ordinal))
                return (false, null, "Correo o contraseña incorrectos.");

            return (false, null, texto);
        }

        var resultado = await response.Content.ReadFromJsonAsync<UsuarioTokenDTO>();
        if (resultado == null || string.IsNullOrWhiteSpace(resultado.Token))
            return (false, null, "La API no devolvió el token.");

        await SetToken(resultado.Token);
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userEmail", login);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", TokenEsAdmin(resultado.Token) ? "ADMIN" : "CLIENTE");
        }
        catch
        {
        }

        return (true, resultado.Token, string.Empty);
    }

    public async Task<(bool Ok, string Mensaje)> RegistrarAsync(string nombre, string login, string clave, string? telefono)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/registro", new UsuarioRegistrarDTO
        {
            Nombre = nombre,
            Login = login,
            Clave = clave,
            Telefono = telefono
        });

        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "Cuenta creada. Ya puedes iniciar sesión.");
    }

    public async Task<(bool Ok, string Mensaje)> SolicitarRecuperacionAsync(string correo)
    {
        AjustarCabeceraAuth(null);
        var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", new RecuperarClaveDTO
        {
            Correo = correo
        });

        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        var cuerpo = await ApiMensaje.Leer(response);
        return (true, string.IsNullOrWhiteSpace(cuerpo)
            ? "Si el correo está registrado, se envió un código."
            : cuerpo);
    }

    public async Task<(bool Ok, string? Token, string Mensaje)> VerificarCodigoAsync(string correo, string codigo)
    {
        AjustarCabeceraAuth(null);
        var response = await _httpClient.PostAsJsonAsync("api/auth/verificar-codigo", new VerificarCodigoDTO
        {
            Correo = correo,
            Codigo = codigo
        });

        if (!response.IsSuccessStatusCode)
            return (false, null, await ApiMensaje.Leer(response));

        var resultado = await response.Content.ReadFromJsonAsync<UsuarioTokenDTO>();
        if (resultado == null || string.IsNullOrWhiteSpace(resultado.Token))
            return (false, null, "La API no confirmó el código.");

        return (true, resultado.Token, string.Empty);
    }

    public async Task<(bool Ok, string Mensaje)> RestablecerClaveAsync(string token, string nuevaClave)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password", new RestablecerClaveDTO
        {
            Token = token,
            NuevaClave = nuevaClave
        });

        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        return (true, "La contraseña se restableció. Inicia sesión de nuevo.");
    }

    public PerfilUsuarioDTO? PerfilEnMemoria => _perfil;

    public Task<PerfilUsuarioDTO?> ObtenerPerfilAsync()
    {
        if (_perfil != null)
            return Task.FromResult<PerfilUsuarioDTO?>(_perfil);

        return _cargaPerfil ??= CargarPerfil();
    }

    private async Task<PerfilUsuarioDTO?> CargarPerfil()
    {
        try
        {
            await GetToken();
            var perfil = await _httpClient.GetFromJsonAsync<PerfilUsuarioDTO>("api/usuarios/perfil");
            _perfil = perfil;
            return perfil;
        }
        catch
        {
            return null;
        }
        finally
        {
            _cargaPerfil = null;
        }
    }

    public async Task<(bool Ok, string Mensaje)> ActualizarPerfilAsync(PerfilUsuarioDTO perfil)
    {
        await GetToken();
        var response = await _httpClient.PutAsJsonAsync("api/usuarios/perfil", new
        {
            nombre = perfil.Nombre,
            correo = perfil.Correo,
            telefono = perfil.Telefono
        });

        if (!response.IsSuccessStatusCode)
            return (false, await ApiMensaje.Leer(response));

        var actualizado = await ApiMensaje.LeerJson<PerfilUsuarioDTO>(response);
        if (actualizado != null)
        {
            perfil.Id = actualizado.Id;
            perfil.Nombre = actualizado.Nombre;
            perfil.Correo = actualizado.Correo;
            perfil.Telefono = actualizado.Telefono;
        }

        _perfil = perfil;

        return (true, "Perfil actualizado.");
    }

    private void AjustarCabeceraAuth(string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
