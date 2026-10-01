using MarketLocalShirts.Components;
using MarketLocalShirts.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CamisaService>();
builder.Services.AddScoped<MarcaService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<RecuperacionEstado>();

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:8080/";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBase) });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();