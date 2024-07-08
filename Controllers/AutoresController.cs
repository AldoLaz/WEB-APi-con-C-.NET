using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{

    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esAdmin")]
    // [Authorize] Filtro
    public class AutoresController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(AplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }


        [HttpGet(Name = "obtenerAutores")]
        [AllowAnonymous]
        public async Task<ActionResult> Get( [FromQuery] bool incluirHATEOAS = true)
        {

            var autores = await context.Autores.ToListAsync();
            var dtos = mapper.Map<List<AutorDTO>>(autores);
            if (incluirHATEOAS)
            {
                var esadmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
                dtos.ForEach(dto => GenerarEnlaces(dto, esadmin.Succeeded));

                var resultado = new ColeccionDeRecursos<AutorDTO> { Valores = dtos };
                resultado.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("obtenerAutores", new { }),
                    descripcion: "Self",
                    metodo: "GET"));

                if (esadmin.Succeeded)
                {
                    resultado.Enlaces.Add(new DatoHATEOAS(
                        enlace: Url.Link("registrarUnAutor", new { }),
                        descripcion: "Registrar-Autor",
                        metodo: "POST"));
                }
                return Ok(resultado);
            }
            return Ok(dtos);

        }

        [HttpGet("id:int",Name = "obtenerAutorPorID")]
        [AllowAnonymous]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores.Include(autorDB => autorDB.AutorLibro).
                ThenInclude(autorLibroDB => autorLibroDB.Libro).
                FirstOrDefaultAsync(autorDB => autorDB.id == id);
            if (autor == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<AutorDTOConLibros>(autor);
            var esadmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            GenerarEnlaces(dto, esadmin.Succeeded);
            return dto;

        }

        private void GenerarEnlaces(AutorDTO autorDTO, bool esAdmin)
        {

            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerAutorPorID", new { id = autorDTO.Id }),
                descripcion: "Self",
                metodo: "GET"));

            if (esAdmin) {
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("modificarUnAutor", new { id = autorDTO.Id }),
                    descripcion: "Actualizar-Autor",
                    metodo: "PUT"));

                autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("borrarAutor", new { id = autorDTO.Id }),
                    descripcion: "Borrar-Autor",
                    metodo: "DELETE"));
            }
        }

        [HttpGet("{nombre}", Name = "obtenerAutoresPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "registrarUnAutor")]
        public async Task<ActionResult> Post(CreacionAutorDTO creacionAutorDTO)
        {

            var existeAutorConElNombre = await context.Autores.AnyAsync(x => x.Nombre == creacionAutorDTO.Nombre);
            if (existeAutorConElNombre)
            {
                return BadRequest($"El nombre {creacionAutorDTO.Nombre} ya existe en la base de datos");
            }

            var autor = mapper.Map<Autor>(creacionAutorDTO);
            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutor",new {id = autor.id},autorDTO); ;
        }

        [HttpPut("id:int", Name = "modificarUnAutor")]
        public async Task<ActionResult> Put(CreacionAutorDTO creacionAutorDTO, int id)
        {
            
            var existe = await context.Autores.AnyAsync(x => x.id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(creacionAutorDTO);
            autor.id = id;
            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("id:int", Name = "borrarAutor")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.id == id);

            if (!existe)
            {
                return NotFound();  
            }
            context.Remove(new Autor() { id = id });
            await context.SaveChangesAsync();  
            return NoContent();    
        }

    }
}
