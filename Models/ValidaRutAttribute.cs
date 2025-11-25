using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GestionBodega.Models
{
    public class ValidaRutAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success; 

            string rut = value.ToString()!.Replace(".", "").Replace("-", "").ToUpper();

            if (rut.Length < 2)
                return new ValidationResult("El RUT ingresado no es válido.");

            string rutCuerpo = rut.Substring(0, rut.Length - 1);
            char dvIngresado = rut[rut.Length - 1];

            if (!int.TryParse(rutCuerpo, out int rutNumerico))
                return new ValidationResult("El formato del RUT no es correcto.");

            int s = 1;
            for (int m = 0; rutNumerico != 0; rutNumerico /= 10)
                s = (s + rutNumerico % 10 * (9 - m++ % 6)) % 11;

            char dvEsperado = (char)(s != 0 ? s + 47 : 75);

            if (dvIngresado == dvEsperado)
                return ValidationResult.Success;
            else
                return new ValidationResult("El RUT es incorrecto o el dígito verificador no coincide.");
        }
    }
}