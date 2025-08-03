using Microsoft.EntityFrameworkCore;
using OrdenesApi.DTOs;
using OrdenesApi.Models;
using OrdenesApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdenesApi.Repositories
{
    /// <summary>
    /// Repositorio espec�fico para la entidad Orden que extiende la funcionalidad
    /// del repositorio gen�rico con operaciones especializadas.
    /// </summary>
    public class OrdenRepository : GenericRepository<Orden>, IOrdenRepository
    {
        public OrdenRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene �rdenes de forma paginada, incluyendo sus productos relacionados.
        /// </summary>
        public async Task<PaginationDTO<Orden>> GetPaginatedAsync(PaginationParams paginationParams)
        {
            // Obtener el total de �rdenes para calcular la paginaci�n
            var totalItems = await _context.Ordenes.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.ElementosPorPagina);
            
            // Obtener las �rdenes de la p�gina solicitada con sus productos
            var ordenes = await _context.Ordenes
                .Include(o => o.OrdenProductos)
                .ThenInclude(op => op.Producto)
                .Skip((paginationParams.NumeroPagina - 1) * paginationParams.ElementosPorPagina)
                .Take(paginationParams.ElementosPorPagina)
                .ToListAsync();

            // Construir el DTO de paginaci�n con los resultados
            return new PaginationDTO<Orden>
            {
                Items = ordenes,
                PaginaActual = paginationParams.NumeroPagina,
                ElementosPorPagina = paginationParams.ElementosPorPagina,
                TotalElementos = totalItems,
                TotalPaginas = totalPages
            };
        }

        /// <summary>
        /// Obtiene una orden por su ID, incluyendo los productos asociados.
        /// </summary>
        public async Task<Orden?> GetOrdenWithProductosAsync(int id)
        {
            return await _context.Ordenes
                .Include(o => o.OrdenProductos)
                .ThenInclude(op => op.Producto)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}