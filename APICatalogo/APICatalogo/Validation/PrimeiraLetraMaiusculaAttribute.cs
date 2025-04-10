using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Validation;

public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(value.ToString()))
        {
            return ValidationResult.Success;
        }
        var primeiraLetra = value.ToString()[0].ToString();
        if(primeiraLetra != primeiraLetra.ToUpper())
        {
            return new ValidationResult("A primeira letra deve ser maiuscula.");
        }

        return ValidationResult.Success;
    }
}
