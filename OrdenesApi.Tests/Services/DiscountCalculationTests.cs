using Microsoft.Extensions.Logging;
using Moq;
using OrdenesApi.Models;
using OrdenesApi.Repositories.Interfaces;
using OrdenesApi.Services;
using System;
using System.Reflection;
using Xunit;

namespace OrdenesApi.Tests.Services
{
    /// <summary>
    /// Pruebas del algoritmo de descuentos:
    /// - 10% si total > $500
    /// - 5% adicional si > 5 productos
    /// </summary>
    public class DiscountCalculationTests
    {
        [Fact]
        public void ValidarCalculoDescuento()
        {
            // Arrange
            var ordenRepo = new Mock<IOrdenRepository>();
            var productoRepo = new Mock<IProductoRepository>();
            var logger = new Mock<ILogger<OrdenService>>();
            
            var service = new OrdenService(ordenRepo.Object, productoRepo.Object, logger.Object);
            
            // Act - Accedemos al método privado mediante reflexión
            var metodoDinamico = typeof(OrdenService).GetMethod(
                "CalcularTotalConDescuento", 
                BindingFlags.NonPublic | BindingFlags.Instance)!;
            
            // Probamos con $600 y 2 productos (solo aplica descuento por monto)
            var resultado = metodoDinamico.Invoke(service, new object[] { 600m, 2 });
            
            // Assert
            Assert.Equal(540m, resultado); // 600 - 10% = 540
        }

        [Fact]
        public void CalcularDescuento_MasDeCincoProductos_AplicaDescuentoAdicional()
        {
            // Arrange
            var ordenRepo = new Mock<IOrdenRepository>();
            var productoRepo = new Mock<IProductoRepository>();
            var logger = new Mock<ILogger<OrdenService>>();
            
            var service = new OrdenService(ordenRepo.Object, productoRepo.Object, logger.Object);
            
            // Act - Accedemos al método privado mediante reflexión
            var metodoDinamico = typeof(OrdenService).GetMethod(
                "CalcularTotalConDescuento", 
                BindingFlags.NonPublic | BindingFlags.Instance)!;
            
            // Probamos con $1000 y 6 productos (aplican ambos descuentos)
            var resultado = metodoDinamico.Invoke(service, new object[] { 1000m, 6 });
            
            // Assert
            Assert.Equal(850m, resultado); // 1000 - 10% - 5% = 850
        }
    }
}