using Microsoft.EntityFrameworkCore;
using OrdenesApi.Models;
using OrdenesApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdenesApi.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Producto que implementa
    /// operaciones adicionales como verificación de existencia y uso.
    /// </summary>
    public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Verifica si un producto existe en la base de datos.
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Productos.AnyAsync(p => p.Id == id);
        }

        /// <summary>
        /// Obtiene múltiples productos por sus IDs.
        /// </summary>
        public async Task<IEnumerable<Producto>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Productos
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        /// <summary>
        /// Verifica si un producto está siendo utilizado en alguna orden.
        /// </summary>
        public async Task<bool> IsProductInUseAsync(int id)
        {
            return await _context.OrdenesProductos.AnyAsync(op => op.ProductoId == id);
        }
    }
}