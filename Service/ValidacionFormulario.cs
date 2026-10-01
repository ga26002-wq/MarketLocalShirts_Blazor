using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.Service;

public static class ValidacionFormulario
{
    public static bool EsValido(object modelo, out string mensaje)
    {
        var errores = Errores(modelo);
        mensaje = errores.Count == 0
            ? string.Empty
            : string.Join(" ", errores.Values);
        return errores.Count == 0;
    }

    public static Dictionary<string, string> Errores(object modelo)
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(modelo, new ValidationContext(modelo), resultados, true);

        var errores = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var resultado in resultados)
        {
            var campo = resultado.MemberNames.FirstOrDefault() ?? string.Empty;
            if (!errores.ContainsKey(campo) && !string.IsNullOrWhiteSpace(resultado.ErrorMessage))
                errores[campo] = resultado.ErrorMessage;
        }

        return errores;
    }
}
