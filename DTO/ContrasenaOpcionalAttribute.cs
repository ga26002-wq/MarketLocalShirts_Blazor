using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO;

public class ContrasenaOpcionalAttribute : ValidationAttribute
{
    public ContrasenaOpcionalAttribute()
    {
        ErrorMessage = "La contrasena debe tener al menos 6 caracteres";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string texto || string.IsNullOrWhiteSpace(texto))
            return true;

        return texto.Trim().Length >= 6;
    }
}
