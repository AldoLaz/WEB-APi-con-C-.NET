using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int id { get; set; }
        [StringLength(maximumLength:250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }  
        public List<Comentarios> Comentario { get; set; }
        public List<AutorLibro> AutorLibro { get; set;}
       
    }
}
