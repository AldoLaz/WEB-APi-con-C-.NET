using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;
namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int id { get; set; }
        [Required(ErrorMessage = "El campo {0} no puede ser nulo o vacio")]//Regla de validacion 
        [StringLength(maximumLength:80, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]//Regla de validacion.
        [PrimeraLetraMayuscula] // Regla de validacion personalizada
        public string Nombre { get; set; }
        public List<AutorLibro> AutorLibro { get; set; }
      
        
    }
}
