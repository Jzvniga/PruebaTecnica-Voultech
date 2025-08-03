using Microsoft.Extensions.Logging;
using OrdenesApi.DTOs; 
using OrdenesApi.Models;
using OrdenesApi.Repositories.Interfaces;
using OrdenesApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdenesApi.Services
{
    /// <summary>
    /// Servicio que implementa la lógica de negocio para gestionar productos,
    /// incluyendo validaciones y verificación de uso en órdenes.
    /// </summary>
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(IProductoRepository productoRepository, ILogger<ProductoService> logger)
        {
            _productoRepository = productoRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los productos disponibles.
        /// </summary>
        public async Task<IEnumerable<ProductoResponseDTO>> GetProductosAsync()
        {
            var productos = await _productoRepository.GetAllAsync();
            return productos.Select(ConvertToDTO).ToList();
        }

        /// <summary>
        /// Obtiene un producto específico por su ID.
        /// </summary>
        public async Task<ProductoResponseDTO> GetProductoAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
                
            return ConvertToDTO(producto);
        }

        /// <summary>
        /// Crea un nuevo producto después de validar sus datos.
        /// </summary>
        public async Task<ProductoResponseDTO> CreateProductoAsync(Producto producto)
        {
            ValidateProducto(producto);
            
            await _productoRepository.AddAsync(producto);
            
            _logger.LogInformation("Producto creado con ID {ProductoId}", producto.Id);
            
            return ConvertToDTO(producto);
        }

        /// <summary>
        /// Actualiza un producto existente después de verificar su existencia y validar sus datos.
        /// </summary>
        public async Task UpdateProductoAsync(int id, Producto producto)
        {
            if (id != producto.Id)
                throw new ArgumentException("El ID del producto no coincide con el ID de la URL");
                
            ValidateProducto(producto);
            
            var exists = await _productoRepository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
                
            await _productoRepository.UpdateAsync(producto);
            
            _logger.LogInformation("Producto actualizado con ID {ProductoId}", id);
        }

        /// <summary>
        /// Elimina un producto si no está siendo utilizado en ninguna orden.
        /// </summary>
        public async Task DeleteProductoAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
                
            // Verificar si el producto está siendo usado en alguna orden
            var productoEnUso = await _productoRepository.IsProductInUseAsync(id);
            
            if (productoEnUso)
                throw new InvalidOperationException("No se puede eliminar el producto porque está siendo utilizado en una o más órdenes");
                
            await _productoRepository.DeleteAsync(id);
            
            _logger.LogInformation("Producto eliminado con ID {ProductoId}", id);
        }
        
        /// <summary>
        /// Verifica si un producto está siendo utilizado en alguna orden.
        /// </summary>
        public async Task<bool> IsProductInUseAsync(int id)
        {
            return await _productoRepository.IsProductInUseAsync(id);
        }

        /// <summary>
        /// Convierte una entidad Producto a su DTO de respuesta.
        /// </summary>
        private ProductoResponseDTO ConvertToDTO(Producto producto)
        {
            return new ProductoResponseDTO
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio
            };
        }
        
        /// <summary>
        /// Valida que los datos del producto sean correctos.
        /// </summary>
        private void ValidateProducto(Producto producto)
        {
            if (string.IsNullOrEmpty(producto.Nombre))
                throw new ArgumentException("El nombre del producto es obligatorio");
                
            if (producto.Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor que cero");
        }
    }
}