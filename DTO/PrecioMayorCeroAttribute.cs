using System.ComponentModel.DataAnnotations;

namespace MarketLocalShirts.DTO;

public class PrecioMayorCeroAttribute : ValidationAttribute
{
    public PrecioMayorCeroAttribute()
    {
        ErrorMessage = "El precio debe ser mayor a 0";
    }

    public override bool IsValid(object? value)
    {
        return value is decimal precio && precio >= 0.01m;
    }
}
