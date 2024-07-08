using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class CreacionAutorDTO
    {
        [Required(ErrorMessage = "El campo {0} no puede ser nulo o vacio")]//Regla de validacion 
        [StringLength(maximumLength: 80, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]//Regla de validacion.
        [PrimeraLetraMayuscula] // Regla de validacion personal

        public string Nombre { get; set; }  
    }
}
