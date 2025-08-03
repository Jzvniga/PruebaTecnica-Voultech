using OrdenesApi.DTOs;
using OrdenesApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdenesApi.Repositories.Interfaces
{
    public interface IOrdenRepository : IGenericRepository<Orden>
    {
        Task<PaginationDTO<Orden>> GetPaginatedAsync(PaginationParams paginationParams);
        Task<Orden?> GetOrdenWithProductosAsync(int id);
    }
}