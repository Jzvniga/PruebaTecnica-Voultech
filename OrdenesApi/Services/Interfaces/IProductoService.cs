using OrdenesApi.DTOs;
using OrdenesApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdenesApi.Services.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoResponseDTO>> GetProductosAsync();
        Task<ProductoResponseDTO> GetProductoAsync(int id);
        Task<ProductoResponseDTO> CreateProductoAsync(Producto producto);
        Task UpdateProductoAsync(int id, Producto producto);
        Task DeleteProductoAsync(int id);
        Task<bool> IsProductInUseAsync(int id);
    }
}