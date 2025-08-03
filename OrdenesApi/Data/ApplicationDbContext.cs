using Microsoft.EntityFrameworkCore;
using OrdenesApi.Models;

namespace OrdenesApi
{
    /// <summary>
    /// Contexto de base de datos para la aplicaci�n de �rdenes.
    /// Configura las entidades y sus relaciones.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Definici�n de tablas en la base de datos
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<OrdenProducto> OrdenesProductos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraci�n de la relaci�n many-to-many entre Orden y Producto
            modelBuilder.Entity<OrdenProducto>()
                .HasOne(op => op.Orden)
                .WithMany(o => o.OrdenProductos)
                .HasForeignKey(op => op.OrdenId);

            modelBuilder.Entity<OrdenProducto>()
                .HasOne(op => op.Producto)
                .WithMany(p => p.OrdenProductos)
                .HasForeignKey(op => op.ProductoId);

            // Configuraci�n de precisi�n para campos decimales
            modelBuilder.Entity<Orden>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);
        }
    }
}