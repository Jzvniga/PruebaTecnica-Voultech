using Microsoft.Extensions.Logging;
using Moq;
using OrdenesApi.DTOs;
using OrdenesApi.Models;
using OrdenesApi.Repositories.Interfaces;
using OrdenesApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OrdenesApi.Tests.Services
{
    public class OrdenServiceTests
    {
        [Fact]
        public async Task CreateOrdenAsync_ConProductosValidos_DebeCrearOrden()
        {
            // Arrange - Configuración inicial de los objetos necesarios para la prueba
            var ordenRepo = new Mock<IOrdenRepository>();
            var productoRepo = new Mock<IProductoRepository>();
            var logger = new Mock<ILogger<OrdenService>>();
            
            // Configuramos el mock del repositorio de productos para que retorne productos de prueba
            // cuando se llame al método GetByIdsAsync con cualquier lista de IDs
            var productos = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Producto 1", Precio = 100 },
                new Producto { Id = 2, Nombre = "Producto 2", Precio = 200 }
            };
            
            productoRepo.Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                        .ReturnsAsync(productos);
            
            // Configuramos el mock del repositorio de órdenes para que
            // cuando se añada una orden, le asigne ID=1 simulando la base de datos
            ordenRepo.Setup(repo => repo.AddAsync(It.IsAny<Orden>()))
                     .ReturnsAsync((Orden orden) => {
                         orden.Id = 1; // Simulamos que la base de datos asigna ID
                         return orden;
                     });
            
            // Creamos el servicio con los mocks configurados
            var service = new OrdenService(ordenRepo.Object, productoRepo.Object, logger.Object);
            
            // Creamos un DTO de prueba para crear una orden
            var ordenDto = new OrdenCreateDTO 
            { 
                Cliente = "Cliente Test",
                ProductosIds = new List<int> { 1, 2 }
            };
            
            // Act - Ejecutamos el método que queremos probar
            var resultado = await service.CreateOrdenAsync(ordenDto);
            
            // Assert - Verificamos que el resultado sea el esperado
            Assert.NotNull(resultado); // La respuesta no debe ser nula
            Assert.Equal("Cliente Test", resultado.Cliente); // El cliente debe mantenerse
            Assert.Equal(1, resultado.Id); // El ID debe ser 1 (asignado en el mock)
            Assert.Equal(300, resultado.Total); // 100 + 200 = 300 (sin descuento aplicable)
            Assert.Equal(2, resultado.Productos.Count); // Deben incluirse los 2 productos
            
            // Verificamos que el método AddAsync del repositorio se llamó exactamente una vez
            ordenRepo.Verify(repo => repo.AddAsync(It.IsAny<Orden>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrdenAsync_SinCliente_DebeLanzarArgumentException()
        {
            // Arrange - Configuración de los mocks y el servicio
            var ordenRepo = new Mock<IOrdenRepository>();
            var productoRepo = new Mock<IProductoRepository>();
            var logger = new Mock<ILogger<OrdenService>>();

            var service = new OrdenService(ordenRepo.Object, productoRepo.Object, logger.Object);

            // Creamos un DTO inválido con cliente vacío para forzar la validación
            var ordenDto = new OrdenCreateDTO
            {
                Cliente = "",  // Cliente vacío - debe fallar la validación
                ProductosIds = new List<int> { 1, 2 }
            };

            // Act & Assert - Verificamos que se lance la excepción esperada
            // Esta sintaxis ejecuta el método y verifica que lance ArgumentException
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await service.CreateOrdenAsync(ordenDto));
        }

        [Fact]
        public async Task UpdateOrdenAsync_OrdenExistente_DebeActualizarOrden()
        {
            // Arrange - Configuración de los mocks y objetos necesarios
            var ordenRepo = new Mock<IOrdenRepository>();
            var productoRepo = new Mock<IProductoRepository>();
            var logger = new Mock<ILogger<OrdenService>>();
                        
            // Configurar el repositorio para devolver una orden existente
            var ordenExistente = new Orden
            {
                Id = 1,
                Cliente = "Cliente Original",
                OrdenProductos = new List<OrdenProducto>()
            };
            
            ordenRepo.Setup(repo => repo.GetOrdenWithProductosAsync(1))
                     .ReturnsAsync(ordenExistente);
            
            // Configurar productos
            var productos = new List<Producto>
            {
                new Producto { Id = 3, Nombre = "Producto 3", Precio = 300 }
            };
            
            productoRepo.Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                        .ReturnsAsync(productos);
            
            var service = new OrdenService(ordenRepo.Object, productoRepo.Object, logger.Object);
            
            // Crear DTO para actualización
            var ordenDto = new OrdenCreateDTO 
            { 
                Cliente = "Cliente Actualizado",
                ProductosIds = new List<int> { 3 }
            };
            
            // Act
            await service.UpdateOrdenAsync(1, ordenDto);
            
            // Assert
            ordenRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Orden>()), Times.Once);
            Assert.Equal("Cliente Actualizado", ordenExistente.Cliente);
            Assert.Single(ordenExistente.OrdenProductos);
        }

        [Fact]
        public async Task GetOrdenAsync_OrdenExistente_DebeRetornarOrden()
        {
            // Arrange - Configuración inicial
            var ordenRepo = new Mock<IOrdenRepository>();
            var productoRepo = new Mock<IProductoRepository>();
            var logger = new Mock<ILogger<OrdenService>>();

            // Creamos una orden completa con productos para simular 
            // lo que devolvería la base de datos
            var ordenExistente = new Orden
            {
                Id = 1,
                Cliente = "Cliente Test",
                FechaCreacion = DateTime.Now,
                Total = 300,
                OrdenProductos = new List<OrdenProducto>
                {
                    new OrdenProducto 
                    { 
                        ProductoId = 1, 
                        Producto = new Producto { Id = 1, Nombre = "Producto 1", Precio = 100 } 
                    },
                    new OrdenProducto 
                    { 
                        ProductoId = 2, 
                        Producto = new Producto { Id = 2, Nombre = "Producto 2", Precio = 200 } 
                    }
                }
            };

            // Configuramos el mock para devolver la orden existente cuando se busque por ID=1
            ordenRepo.Setup(repo => repo.GetOrdenWithProductosAsync(1))
                     .ReturnsAsync(ordenExistente);

            var service = new OrdenService(ordenRepo.Object, productoRepo.Object, logger.Object);
            
            // Act - Ejecutamos el método que queremos probar
            var resultado = await service.GetOrdenAsync(1);
            
            // Assert - Verificamos que el resultado contiene los datos esperados
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Cliente Test", resultado.Cliente);
            Assert.Equal(300, resultado.Total);
            Assert.Equal(2, resultado.Productos.Count); // Debe contener los 2 productos
        }
    }
}