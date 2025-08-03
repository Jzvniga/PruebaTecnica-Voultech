using System;

namespace OrdenesApi.DTOs
{
    /// <summary>
    /// DTO para respuestas de productos en las operaciones de la API.
    /// </summary>
    public class ProductoResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }
}