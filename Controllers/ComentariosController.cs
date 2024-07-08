using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{

    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/libros/{libroId:int}/comnetarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(AplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "obtenerComentarioPorIDLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var comentarios = await context.Comentarios.Where(comentarioDB => comentarioDB.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);

        }

        [HttpGet("id:int", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetPorId(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }
            return mapper.Map<ComentarioDTO>(comentario);

        }


        [HttpPost(Name = "agregarComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, CreacionComentariosDTO creacionComentariosDTO)
        {
            var claimEmail =  HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = claimEmail.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentarios>(creacionComentariosDTO);
            comentario.LibroId = libroId;
            comentario.UsuaioId = usuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("ObtenerComentario", new { Id = comentario.Id, libroId = libroId }, comentarioDTO);
           
        }

        [HttpPut("id:int", Name = "modificarComentario")]
        public async Task<ActionResult> Put(int Id, int libroId, CreacionComentariosDTO creacionComentariosDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.id == libroId );

            if (!existeLibro)
            {
                return NotFound();
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentariosDB => comentariosDB.Id == Id);

            if (!existeComentario)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentarios>(creacionComentariosDTO);
            comentario.Id = Id;
            comentario.LibroId = libroId;
            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();

        }

     
    }
}
