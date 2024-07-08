using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController: ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService) 
        {
            this.authorizationService = authorizationService;
        }
        [HttpGet(Name= "ObtenerRoot")]

        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datoHateoas = new List<DatoHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "Get"));

            datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "Obtener-Autores", metodo: "Get"));

            if (esAdmin.Succeeded)
            {
                datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("registrarUnAutor", new { }), descripcion: "Registrar-Autor", metodo: "Post"));
                datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("agregarLibro", new { }), descripcion: "Agregar-Libro", metodo: "Post"));
            }
            

            return datoHateoas;
        }







    }
}
