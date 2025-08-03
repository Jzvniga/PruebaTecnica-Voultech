using OrdenesApi.DTOs;
using OrdenesApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdenesApi.Services.Interfaces
{
    public interface IOrdenService
    {
        Task<PaginationDTO<OrdenResponseDTO>> GetOrdenesAsync(PaginationParams paginationParams);
        Task<OrdenResponseDTO> GetOrdenAsync(int id);
        Task<OrdenResponseDTO> CreateOrdenAsync(OrdenCreateDTO ordenDto);
        Task UpdateOrdenAsync(int id, OrdenCreateDTO ordenDto);
        Task DeleteOrdenAsync(int id);
    }
}