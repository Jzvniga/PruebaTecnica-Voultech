using Microsoft.AspNetCore.Mvc;
using OrdenesApi.DTOs;
using OrdenesApi.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdenesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdenesController : ControllerBase
    {
        private readonly IOrdenService _ordenService;
        private readonly ILogger<OrdenesController> _logger;

        public OrdenesController(IOrdenService ordenService, ILogger<OrdenesController> logger)
        {
            _ordenService = ordenService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las órdenes con paginación
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginationDTO<OrdenResponseDTO>>> GetOrdenes([FromQuery] PaginationParams paginationParams)
        {
            return await _ordenService.GetOrdenesAsync(paginationParams);
        }

        /// <summary>
        /// Obtiene una orden específica por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrdenResponseDTO>> GetOrden(int id)
        {
            try
            {
                return await _ordenService.GetOrdenAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Crea una nueva orden
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrdenResponseDTO>> PostOrden(OrdenCreateDTO ordenDto)
        {
            try
            {
                var orden = await _ordenService.CreateOrdenAsync(ordenDto);
                return CreatedAtAction(nameof(GetOrden), new { id = orden.Id }, orden);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una orden existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutOrden(int id, OrdenCreateDTO ordenDto)
        {
            try
            {
                await _ordenService.UpdateOrdenAsync(id, ordenDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina una orden
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrden(int id)
        {
            try
            {
                await _ordenService.DeleteOrdenAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}