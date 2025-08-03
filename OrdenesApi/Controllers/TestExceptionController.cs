using Microsoft.AspNetCore.Mvc;
using System;

namespace OrdenesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestExceptionController : ControllerBase
    {
        /// <summary>
        /// Endpoint para probar excepción genérica
        /// </summary>
        [HttpGet("error")]
        public IActionResult GetError()
        {
            throw new Exception("Esta es una excepción de prueba");
        }

        /// <summary>
        /// Endpoint para probar excepción de argumento
        /// </summary>
        [HttpGet("argument-error")]
        public IActionResult GetArgumentError()
        {
            throw new ArgumentException("Error en los argumentos proporcionados");
        }

        /// <summary>
        /// Endpoint para probar excepción no controlada
        /// </summary>
        [HttpGet("null-reference")]
        public IActionResult GetNullReferenceError()
        {
            // Corrección para evitar CS8600 (Converting null literal) y CS8602 (Dereference of possibly null reference)
            string? s = null;
            if (s == null)
                return BadRequest("s es null");
                
            return Ok(s.Length);
        }
    }
}