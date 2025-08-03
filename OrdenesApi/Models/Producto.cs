namespace OrdenesApi.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }

        public ICollection<OrdenProducto> OrdenProductos { get; set; } = new List<OrdenProducto>();
    }
}