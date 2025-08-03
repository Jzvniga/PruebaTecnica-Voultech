using System;
using System.Collections.Generic;

namespace OrdenesApi.DTOs
{
    /// <summary>
    /// DTO para respuestas de �rdenes con informaci�n completa, incluyendo productos
    /// </summary>
    public class OrdenResponseDTO
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public decimal Total { get; set; }
        public List<ProductoSimpleDTO> Productos { get; set; } = new List<ProductoSimpleDTO>();
    }

    /// <summary>
    /// Versi�n simplificada del producto para incluir en respuestas de �rdenes
    /// </summary>
    public class ProductoSimpleDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }

    /// <summary>
    /// DTO para crear nuevas �rdenes, solo requiere cliente y lista de IDs de productos
    /// </summary>
    public class OrdenCreateDTO
    {
        public string Cliente { get; set; } = string.Empty;
        public List<int> ProductosIds { get; set; } = new List<int>();
    }
}