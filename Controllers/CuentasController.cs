using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashServices hashServices;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider, HashServices hashServices) 
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashServices = hashServices;
            dataProtector = dataProtectionProvider.CreateProtector("Llave_Nibiru_24");
        }

        //[HttpGet("hash/{textoPlano}")]
        //public ActionResult RealizarHash(string textoPlano)
        //{
        //    var resultado1 = hashServices.Hash(textoPlano);
        //    var resultado2 = hashServices.Hash(textoPlano);

        //    return Ok(new {
        //        textoPlano = textoPlano,
        //        resultado1 = resultado1,
        //        resultado2 = resultado2,   
        //    });
        //}

        //[HttpGet("Encriptar")]
        //public ActionResult Encriptar()
        //{
        //    var textoPlano = " Aldo Hernandez ";
        //    var textoCifrado = dataProtector.Protect(textoPlano);
        //    var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

        //    return Ok(new {
        //        textoPlano = textoPlano,
        //        textoCifrado = textoCifrado,
        //        textoDesencriptado = textoDesencriptado

        //    });

            
        //}

        //[HttpGet("EncriptarPorTiempo")]
        //public ActionResult EncriptarPorTiempo()
        //{
        //    var protectorPorTiempo = dataProtector.ToTimeLimitedDataProtector();
        //    var textoPlano = " Aldo Hernandez ";
        //    var textoCifrado = protectorPorTiempo.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
        //    Thread.Sleep(TimeSpan.FromSeconds(6));
        //    var textoDesencriptado = protectorPorTiempo.Unprotect(textoCifrado);

        //    return Ok(new
        //    {
        //        textoPlano = textoPlano,
        //        textoCifrado = textoCifrado,
        //        textoDesencriptado = textoDesencriptado

        //    });


        //}

        [HttpPost("Registrar", Name = "registrarUsuario")]
        public async Task<ActionResult<RegistrarAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {

            var usuario = new IdentityUser { UserName = credencialesUsuario.Email, Email = credencialesUsuario .Email };

            var resultado = await userManager.CreateAsync(usuario,credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }

        }

        [HttpPost("login", Name = "iniciarSesion")]
        public async Task<ActionResult<RegistrarAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {

            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Credenciales no validas");
            }

        }

        [HttpGet("RenovarToken", Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RegistrarAutenticacion>> Renovar()
        {
            var claimEmail = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = claimEmail.Value;
            var credencialesUsuario = new CredencialesUsuario() {
                Email = email
            };
            return await ConstruirToken(credencialesUsuario);
        }

       private async Task<RegistrarAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email)
            };
            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RegistrarAutenticacion()
            {
            
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion

            };
            
            
        }

        [HttpPost("HacerAdmin",Name = "hacerAdminAUsuario")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin","True"));
            return NoContent();
        }

        [HttpPost("RemoverAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "True"));
            return NoContent();
        }

    }
}
