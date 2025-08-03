    using Microsoft.EntityFrameworkCore;
using OrdenesApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdenesApi.Repositories
{
    /// <summary>
    /// Implementación genérica del patrón repositorio que proporciona
    /// operaciones CRUD básicas para cualquier entidad.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las entidades del tipo especificado.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Busca una entidad por su ID.
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Agrega una nueva entidad a la base de datos.
        /// </summary>
        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) // Corregido para CS8604
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}