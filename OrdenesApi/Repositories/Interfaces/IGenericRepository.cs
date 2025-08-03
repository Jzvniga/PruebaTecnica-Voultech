using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdenesApi.Repositories.Interfaces
{
    // Modifica la interfaz para que coincida con la implementaci�n
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);  // A�ade "?" aqu�
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}   