using System;

namespace OrdenesApi.DTOs
{
    /// <summary>
    /// DTO b�sico para transferencia de datos de productos.
    /// </summary>
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }
}