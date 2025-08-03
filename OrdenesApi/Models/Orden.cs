using System;
using System.Collections.Generic;

namespace OrdenesApi.Models
{
    public class Orden
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = null!;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }

        public ICollection<OrdenProducto> OrdenProductos { get; set; } = new List<OrdenProducto>();
    }
}