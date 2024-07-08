using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreacionAutorDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>().ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros)); 

            CreateMap<CreacionLibroDTO, Libro>().ForMember(libro => libro.AutorLibro, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>().ForMember(libroDTO => libroDTO.Autores,opciones => opciones.MapFrom(MapLibroDTOAutores));

            CreateMap<LibroPatchDTO, Libro>().ReverseMap();

            CreateMap<CreacionComentariosDTO, Comentarios>();
            CreateMap<Comentarios, ComentarioDTO>();
        }
        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();

            if (autor.AutorLibro == null) { return resultado; }

            foreach (var autorLibro in autor.AutorLibro)
            {
                resultado.Add(new LibroDTO()
                {
                    id = autorLibro.libroID,
                    Titulo = autorLibro.Libro.Titulo
                });
            }

            return resultado;
        }
        
        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();  
            if (libro.AutorLibro == null) { return resultado; }
            foreach (var autorlibro in libro.AutorLibro)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorlibro.autorID,
                    Nombre = autorlibro.Autor.Nombre               
                });
            }
            return resultado;
        }
        private List<AutorLibro> MapAutoresLibros(CreacionLibroDTO creacionLibroDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if (creacionLibroDTO.AutoresIds == null)
            {
                return resultado;
            }
            foreach (var autorId in creacionLibroDTO.AutoresIds)
            {
                    resultado.Add(new AutorLibro() { autorID = autorId });
            }
                return resultado;
            
        }
    }
}
