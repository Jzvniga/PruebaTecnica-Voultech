using OrdenesApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdenesApi.Repositories.Interfaces
{
    public interface IProductoRepository : IGenericRepository<Producto>
    {
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Producto>> GetByIdsAsync(IEnumerable<int> ids);
        Task<bool> IsProductInUseAsync(int id);
    }
}