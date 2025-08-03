using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;

namespace OrdenesApi.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Registrar la excepción
            _logger.LogError(context.Exception, "Error no controlado: {Message}", context.Exception.Message);

            // Configuración por defecto
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "Se ha producido un error interno en el servidor";

            // Personalizar la respuesta según el tipo de excepción
            if (context.Exception is DbUpdateException dbUpdateEx)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = "Error al actualizar la base de datos";

                if (dbUpdateEx.InnerException != null)
                {
                    _logger.LogError(dbUpdateEx.InnerException, "Error detallado de DB");
                }
            }
            else if (context.Exception is DbUpdateConcurrencyException)
            {
                statusCode = StatusCodes.Status409Conflict;
                message = "El recurso ha sido modificado por otro usuario";
            }
            else if (context.Exception is ArgumentNullException || context.Exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = context.Exception.Message;
            }

            // Crear una respuesta JSON con información del error
            var response = new
            {
                error = message,
                details = context.Exception.Message
            };

            // Configurar el resultado de la respuesta
            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };

            // Marcar la excepción como manejada
            context.ExceptionHandled = true;
        }
    }
}