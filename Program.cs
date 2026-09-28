using MarketLocalShirts.Components;
using MarketLocalShirts.Service.Auth;
using MarketLocalShirts.Service.Carrito;
using MarketLocalShirts.Service.Categoria;
using MarketLocalShirts.Service.Pedido;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar Servicios de la aplicación
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<PedidoService>();

builder.Services.AddServerSideBlazor();

// Configurar HttpClient apuntando a tu API Spring Boot
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:8080/") });

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