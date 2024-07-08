using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace WebApiAutores.Filtros
{
    public class MiFiltro : IActionFilter
    {
        private readonly ILogger<MiFiltro> logger;

        public MiFiltro(ILogger<MiFiltro>logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Antes de Ejecutar la accion");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Despues de Ejecutar la accion");
        }

        
    }
}
