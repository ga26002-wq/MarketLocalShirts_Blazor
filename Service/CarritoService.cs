using MarketLocalShirts.DTO.CamisaDTO;
using MarketLocalShirts.DTO.CarritoDTO;
using MarketLocalShirts.DTO.PedidoDTO;

namespace MarketLocalShirts.Service;

public class CarritoService
{
    private readonly PedidoService _pedidoService;
    private readonly List<CarritoDTO> _items = new();
    private int _siguienteId = 1;

    public CarritoService(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    public Task AgregarAsync(CamisaSalidaDTO camisa, string talla, int cantidad)
    {
        var existente = _items.FirstOrDefault(x => x.CamisaId == camisa.Id && x.Talla == talla);
        if (existente != null)
            existente.Cantidad += cantidad;
        else
        {
            _items.Add(new CarritoDTO
            {
                Id = _siguienteId++,
                CamisaId = camisa.Id,
                NombreProducto = camisa.Nombre,
                Talla = talla,
                Precio = camisa.Precio,
                Cantidad = cantidad
            });
        }

        return Task.CompletedTask;
    }

    public Task<List<CarritoDTO>> ObtenerAsync()
    {
        return Task.FromResult(_items.ToList());
    }

    public Task ActualizarCantidadAsync(int idItem, int nuevaCantidad)
    {
        var item = _items.FirstOrDefault(x => x.Id == idItem);
        if (item != null && nuevaCantidad >= 1)
            item.Cantidad = nuevaCantidad;

        return Task.CompletedTask;
    }

    public Task EliminarAsync(int idItem)
    {
        _items.RemoveAll(x => x.Id == idItem);
        return Task.CompletedTask;
    }

    public async Task<(bool Ok, string Mensaje)> PagarAsync()
    {
        if (_items.Count == 0)
            return (false, "El carrito está vacío.");

        var pedido = new PedidoGuardarDTO
        {
            Detalles = _items.Select(item => new DetallePedidoGuardarDTO
            {
                CamisaId = item.CamisaId,
                Talla = item.Talla,
                Cantidad = item.Cantidad
            }).ToList()
        };

        if (!ValidacionFormulario.EsValido(pedido, out var errorPedido))
            return (false, errorPedido);

        foreach (var detalle in pedido.Detalles)
        {
            if (!ValidacionFormulario.EsValido(detalle, out var errorDetalle))
                return (false, errorDetalle);
        }

        var resultado = await _pedidoService.CrearAsync(pedido);
        if (resultado.Ok)
            _items.Clear();

        return resultado;
    }
}
