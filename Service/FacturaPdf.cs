using System.Globalization;
using System.Text;
using MarketLocalShirts.DTO.PedidoDTO;

namespace MarketLocalShirts.Service;

public static class FacturaPdf
{
    public static byte[] Crear(PedidoSalidaDTO pedido, string nombre, string correo, string telefono)
    {
        var contenido = Dibujar(pedido, nombre, correo, telefono);
        var bytes = Encoding.ASCII.GetBytes(contenido);
        using var ms = new MemoryStream();

        void Escribir(string texto)
        {
            var datos = Encoding.ASCII.GetBytes(texto);
            ms.Write(datos, 0, datos.Length);
        }

        Escribir("%PDF-1.4\n");
        var offsets = new List<long> { 0 };

        void Objeto(string cuerpo)
        {
            offsets.Add(ms.Position);
            Escribir($"{offsets.Count - 1} 0 obj\n{cuerpo}\nendobj\n");
        }

        Objeto("<< /Type /Catalog /Pages 2 0 R >>");
        Objeto("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
        Objeto("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R /F2 6 0 R >> >> >>");
        offsets.Add(ms.Position);
        Escribir($"4 0 obj\n<< /Length {bytes.Length} >>\nstream\n");
        ms.Write(bytes, 0, bytes.Length);
        Escribir("\nendstream\nendobj\n");
        Objeto("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
        Objeto("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");

        var inicio = ms.Position;
        Escribir($"xref\n0 {offsets.Count}\n");
        Escribir("0000000000 65535 f \n");
        for (var i = 1; i < offsets.Count; i++)
            Escribir($"{offsets[i]:D10} 00000 n \n");

        Escribir($"trailer\n<< /Size {offsets.Count} /Root 1 0 R >>\nstartxref\n{inicio}\n%%EOF");
        return ms.ToArray();
    }

    private static string Dibujar(PedidoSalidaDTO pedido, string nombre, string correo, string telefono)
    {
        var sb = new StringBuilder();
        var cultura = CultureInfo.InvariantCulture;
        var numero = $"{pedido.Fecha:yyyy}-{pedido.Id:D4}";
        var porcentaje = pedido.Subtotal <= 0
            ? "13"
            : Math.Round(pedido.Iva / pedido.Subtotal * 100m, 0).ToString("0", cultura);

        void Texto(float x, float y, string valor, float tamano, bool azul, bool negrita)
        {
            var fuente = negrita ? "/F2" : "/F1";
            var color = azul ? "0.18 0.42 0.96" : "0.12 0.12 0.12";
            sb.Append(color).Append(" rg\nBT\n")
                .Append(fuente).Append(' ').Append(tamano.ToString(cultura)).Append(" Tf\n")
                .Append("1 0 0 1 ").Append(x.ToString(cultura)).Append(' ').Append(y.ToString(cultura)).Append(" Tm\n(")
                .Append(Escapar(valor)).Append(") Tj\nET\n");
        }

        void Linea(float x1, float x2, float y)
        {
            sb.Append("0.82 0.84 0.88 RG\n0.6 w\n")
                .Append(x1.ToString(cultura)).Append(' ').Append(y.ToString(cultura)).Append(" m\n")
                .Append(x2.ToString(cultura)).Append(' ').Append(y.ToString(cultura)).Append(" l\nS\n");
        }

        Texto(48, 790, "Factura", 26, true, true);
        Texto(330, 790, "MARKETLOCALSHIRTS", 16, true, true);
        Texto(48, 748, "Fecha de factura:", 11, false, true);
        Texto(170, 748, pedido.Fecha.ToString("dd/MM/yyyy"), 11, false, false);
        Texto(48, 730, "Numero de factura:", 11, false, true);
        Texto(170, 730, numero, 11, false, false);
        Texto(48, 712, "Estado:", 11, false, true);
        Texto(170, 712, pedido.EstadoTexto, 11, false, false);

        Texto(48, 670, string.IsNullOrWhiteSpace(nombre) ? "Cliente" : nombre, 12, true, true);
        Texto(320, 670, "MarketLocalShirts", 12, true, true);
        Texto(48, 650, "Correo: " + (string.IsNullOrWhiteSpace(correo) ? "—" : correo), 10, false, false);
        Texto(320, 650, "Tienda en linea de camisas", 10, false, false);
        Texto(48, 634, "Telefono: " + (string.IsNullOrWhiteSpace(telefono) ? "—" : telefono), 10, false, false);
        Texto(320, 634, "Pedido #" + pedido.Id, 10, false, false);

        Texto(48, 590, "Descripcion", 11, true, true);
        Texto(300, 590, "Unidades", 11, true, true);
        Texto(390, 590, "Precio unitario", 11, true, true);
        Texto(500, 590, "Precio", 11, true, true);
        Linea(48, 547, 582);

        var y = 562f;
        foreach (var detalle in pedido.Detalles ?? new List<DetallePedidoSalidaDTO>())
        {
            var descripcion = detalle.NombreProducto;
            if (!string.IsNullOrWhiteSpace(detalle.Talla))
                descripcion += "  Talla " + detalle.Talla;

            Texto(48, y, descripcion, 11, false, false);
            Texto(320, y, detalle.Cantidad.ToString(cultura), 11, false, false);
            Texto(410, y, "$" + detalle.PrecioUnitario.ToString("F2", cultura), 11, false, false);
            Texto(500, y, "$" + detalle.SubTotal.ToString("F2", cultura), 11, false, false);
            Linea(48, 547, y - 8);
            y -= 28;
            if (y < 180)
                break;
        }

        y -= 16;
        Texto(330, y, "BASE IMPONIBLE:", 11, true, true);
        Texto(490, y, "$" + pedido.Subtotal.ToString("F2", cultura), 11, false, false);
        y -= 20;
        Texto(330, y, "IVA (" + porcentaje + " %):", 11, true, true);
        Texto(490, y, "$" + pedido.Iva.ToString("F2", cultura), 11, false, false);
        y -= 24;
        Texto(330, y, "TOTAL:", 13, true, true);
        Texto(470, y, "$" + pedido.Total.ToString("F2", cultura), 14, true, true);

        y -= 50;
        Texto(48, y, "Comentarios:", 12, true, true);
        y -= 18;
        Texto(48, y, "Pedido pagado. Gracias por su compra.", 11, false, false);

        return sb.ToString();
    }

    private static string Escapar(string? valor)
    {
        if (string.IsNullOrEmpty(valor))
            return string.Empty;

        var sb = new StringBuilder();
        foreach (var caracter in valor)
        {
            sb.Append(caracter switch
            {
                '\\' => "\\\\",
                '(' => "\\(",
                ')' => "\\)",
                'á' => "\\341",
                'é' => "\\351",
                'í' => "\\355",
                'ó' => "\\363",
                'ú' => "\\372",
                'ñ' => "\\361",
                'Á' => "\\301",
                'É' => "\\311",
                'Í' => "\\315",
                'Ó' => "\\323",
                'Ú' => "\\332",
                'Ñ' => "\\321",
                'ü' => "\\374",
                'Ü' => "\\334",
                _ when caracter < ' ' || caracter > '~' => "?",
                _ => caracter.ToString()
            });
        }

        return sb.ToString();
    }
}
