using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class LibroDTO
    {

        public int id { get; set; }
        public DateTime? FechaPublicacion { get; set; } 
        public string Titulo { get; set; }
                 
    }
}
