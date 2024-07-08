using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using System.Collections.Generic;
using AutoMapper;
using WebApiAutores.DTOs;
using Azure;
using Microsoft.AspNetCore.JsonPatch;


namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController: ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(AplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("id:int", Name ="obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await context.Libros.
                Include(LibroDB => LibroDB.AutorLibro).
                ThenInclude(AutorLibroDB => AutorLibroDB.Autor).
                FirstOrDefaultAsync(LibroDB => LibroDB.id == id);

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutorLibro = libro.AutorLibro.OrderBy(x => x.orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost(Name = "agregarLibro")]
        public async Task<ActionResult> Post(CreacionLibroDTO creacionLibroDTO)
        {
            if (creacionLibroDTO.AutoresIds == null)
            {
                return BadRequest("No puedes crear un libro sin agregar autores");
            }

            var existeIdsAutor = await context.Autores.Where(AutorDB => 
            creacionLibroDTO.AutoresIds.Contains(AutorDB.id)).Select(x => x.id).ToListAsync();

            if (creacionLibroDTO.AutoresIds.Count != existeIdsAutor.Count)
            {
                return BadRequest("Uno de los autores no esta registrado en la base de datos");
            }

            var libro = mapper.Map<Libro>(creacionLibroDTO);

            AsignarOrdenAutores(libro);



            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("obtenerLibro",new { Id = libro.id }, libroDTO);

        }

        [HttpPut(Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, CreacionLibroDTO creacionLibroDTO)
        {
            var libroDB = await context.Libros.Include(x => x.AutorLibro).FirstOrDefaultAsync(x => x.id == id);
            if (libroDB == null )
            {
                return NotFound();
            }

            libroDB = mapper.Map(creacionLibroDTO,libroDB);
            AsignarOrdenAutores(libroDB);

           await context.SaveChangesAsync();
            return NoContent();

        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutorLibro != null)
            {
                for (int i = 0; i < libro.AutorLibro.Count; i++)
                {
                    libro.AutorLibro[i].orden = i;
                }

            }
        }

        [HttpPatch("id:int", Name = "buscarLibroPorID")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.id == id);

            if (libroDB == null)
            {
                return NotFound();
            }
            var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);

            patchDocument.ApplyTo(libroDTO, ModelState);
            var Esvalido = TryValidateModel(libroDTO);

            if (!Esvalido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO,libroDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("id:int", Name = "borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.id == id);

            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Libro() { id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
