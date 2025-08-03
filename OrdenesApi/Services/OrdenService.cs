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
    /// Servicio que implementa la lógica de negocio para gestionar órdenes,
    /// incluyendo el algoritmo personalizado de descuentos.
    /// </summary>
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ILogger<OrdenService> _logger;

        public OrdenService(
            IOrdenRepository ordenRepository, 
            IProductoRepository productoRepository,
            ILogger<OrdenService> logger)
        {
            _ordenRepository = ordenRepository;
            _productoRepository = productoRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene órdenes de forma paginada con sus productos.
        /// </summary>
        public async Task<PaginationDTO<OrdenResponseDTO>> GetOrdenesAsync(PaginationParams paginationParams)
        {
            var paginatedOrdenes = await _ordenRepository.GetPaginatedAsync(paginationParams);

            var ordenesDTO = paginatedOrdenes.Items.Select(ConvertToDTO).ToList();

            _logger.LogInformation("Retornando página {NumeroPagina} de {TotalPaginas} de órdenes", 
                paginatedOrdenes.PaginaActual, paginatedOrdenes.TotalPaginas);

            return new PaginationDTO<OrdenResponseDTO>
            {
                Items = ordenesDTO,
                PaginaActual = paginatedOrdenes.PaginaActual,
                ElementosPorPagina = paginatedOrdenes.ElementosPorPagina,
                TotalElementos = paginatedOrdenes.TotalElementos,
                TotalPaginas = paginatedOrdenes.TotalPaginas
            };
        }

        /// <summary>
        /// Obtiene una orden específica por su ID.
        /// </summary>
        public async Task<OrdenResponseDTO> GetOrdenAsync(int id)
        {
            var orden = await _ordenRepository.GetOrdenWithProductosAsync(id);
            if (orden == null)
                throw new KeyNotFoundException($"Orden con ID {id} no encontrada");

            return ConvertToDTO(orden);
        }

        /// <summary>
        /// Crea una nueva orden con sus productos asociados y aplica descuentos.
        /// </summary>
        public async Task<OrdenResponseDTO> CreateOrdenAsync(OrdenCreateDTO ordenDto)
        {
            ValidateOrdenDTO(ordenDto);

            // Verificar que todos los productos existan
            var productosIds = ordenDto.ProductosIds;
            var productos = await _productoRepository.GetByIdsAsync(productosIds);
            
            if (productos.Count() != productosIds.Count())
                throw new ArgumentException("Uno o más productos no existen");

            // Crear la orden
            var orden = new Orden
            {
                Cliente = ordenDto.Cliente,
                FechaCreacion = DateTime.Now,
                OrdenProductos = new List<OrdenProducto>()
            };

            // Agregar productos a la orden
            foreach (var productoId in productosIds)
            {
                orden.OrdenProductos.Add(new OrdenProducto
                {
                    ProductoId = productoId,
                    Orden = orden
                });
            }

            // Calcular subtotal
            decimal subtotal = productos.Sum(p => p.Precio);

            // Aplicar descuentos
            orden.Total = CalcularTotalConDescuento(subtotal, productos.Count());

            await _ordenRepository.AddAsync(orden);
            
            _logger.LogInformation("Orden creada con ID {OrdenId}", orden.Id);

            // Crear DTO de respuesta
            var ordenResponseDTO = new OrdenResponseDTO
            {
                Id = orden.Id,
                Cliente = orden.Cliente,
                FechaCreacion = orden.FechaCreacion,
                Total = orden.Total,
                Productos = productos.Select(p => new ProductoSimpleDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Precio = p.Precio
                }).ToList()
            };
            
            return ordenResponseDTO;
        }

        /// <summary>
        /// Actualiza una orden existente y recalcula descuentos.
        /// </summary>
        public async Task UpdateOrdenAsync(int id, OrdenCreateDTO ordenDto)
        {
            ValidateOrdenDTO(ordenDto);

            var orden = await _ordenRepository.GetOrdenWithProductosAsync(id);
            if (orden == null)
                throw new KeyNotFoundException($"Orden con ID {id} no encontrada");

            // Verificar que todos los productos existan
            var productosIds = ordenDto.ProductosIds;
            var productos = await _productoRepository.GetByIdsAsync(productosIds);

            if (productos.Count() != productosIds.Count())
                throw new ArgumentException("Uno o más productos no existen");

            // Actualizar la orden
            orden.Cliente = ordenDto.Cliente;

            // Eliminar productos anteriores
            orden.OrdenProductos.Clear();

            // Agregar nuevos productos
            foreach (var productoId in productosIds)
            {
                orden.OrdenProductos.Add(new OrdenProducto
                {
                    OrdenId = id,
                    ProductoId = productoId
                });
            }

            // Calcular subtotal
            decimal subtotal = productos.Sum(p => p.Precio);

            // Aplicar descuentos
            orden.Total = CalcularTotalConDescuento(subtotal, productos.Count());

            await _ordenRepository.UpdateAsync(orden);
            
            _logger.LogInformation("Orden actualizada con ID {OrdenId}", id);
        }

        /// <summary>
        /// Elimina una orden por su ID.
        /// </summary>
        public async Task DeleteOrdenAsync(int id)
        {
            var orden = await _ordenRepository.GetOrdenWithProductosAsync(id);
            if (orden == null)
                throw new KeyNotFoundException($"Orden con ID {id} no encontrada");

            await _ordenRepository.DeleteAsync(id);
            
            _logger.LogInformation("Orden eliminada con ID {OrdenId}", id);
        }

        /// <summary>
        /// Valida que el DTO de orden tenga los datos requeridos.
        /// </summary>
        private void ValidateOrdenDTO(OrdenCreateDTO ordenDto)
        {
            if (string.IsNullOrEmpty(ordenDto.Cliente))
                throw new ArgumentException("El nombre del cliente es obligatorio");

            if (ordenDto.ProductosIds == null || !ordenDto.ProductosIds.Any())
                throw new ArgumentException("Debe incluir al menos un producto en la orden");
        }

        /// <summary>
        /// Convierte una entidad Orden a su DTO de respuesta.
        /// </summary>
        private OrdenResponseDTO ConvertToDTO(Orden orden)
        {
            return new OrdenResponseDTO
            {
                Id = orden.Id,
                Cliente = orden.Cliente,
                FechaCreacion = orden.FechaCreacion,
                Total = orden.Total,
                Productos = orden.OrdenProductos.Select(op => new ProductoSimpleDTO
                {
                    Id = op.Producto.Id,
                    Nombre = op.Producto.Nombre,
                    Precio = op.Producto.Precio
                }).ToList()
            };
        }

        /// <summary>
        /// Algoritmo de descuentos personalizado:
        /// - 10% si el total supera $500
        /// - 5% adicional si hay más de 5 productos distintos
        /// </summary>  
        private decimal CalcularTotalConDescuento(decimal subtotal, int cantidadProductosDistintos)
        {
            decimal descuento = 0;

            // Si el total supera $500, aplicar 10% de descuento
            if (subtotal > 500)
            {
                descuento += subtotal * 0.10m;
            }

            // Si hay más de 5 productos distintos, aplicar 5% adicional
            if (cantidadProductosDistintos > 5)
            {
                descuento += subtotal * 0.05m;
            }

            return subtotal - descuento;
        }
    }
}