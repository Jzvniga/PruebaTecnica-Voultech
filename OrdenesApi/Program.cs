using Microsoft.EntityFrameworkCore;
using OrdenesApi;
using OrdenesApi.Models;
using Microsoft.OpenApi.Models;
using OrdenesApi.Filters;
using OrdenesApi.Repositories.Interfaces;
using OrdenesApi.Repositories;
using OrdenesApi.Services.Interfaces;
using OrdenesApi.Services;

// Punto de entrada principal de la aplicación ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS para permitir solicitudes desde cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configura la aplicación para escuchar en puertos específicos
builder.WebHost.UseUrls("http://localhost:5062", "https://localhost:7062");

// Configuración de Entity Framework Core con SQL Server LocalDB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OrdenesDb;Trusted_Connection=True;"));

// Registro de controladores con filtro global de excepciones y configuración JSON
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // Manejo centralizado de excepciones
})
.AddJsonOptions(options =>
{
    // Evita referencias circulares en la serialización JSON
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Habilita la exploración de endpoints de API
builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger para documentación de la API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrdenesApi",
        Version = "v1",
        Description = "API para gestión de órdenes y productos"
    });
});

// Registro de repositorios en el contenedor de dependencias
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Registro de servicios en el contenedor de dependencias
builder.Services.AddScoped<IOrdenService, OrdenService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// Construcción de la aplicación
var app = builder.Build();

// Habilita CORS con la política configurada
app.UseCors("AllowAll");

// Configuración del middleware de Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrdenesApi V1");
    c.RoutePrefix = "swagger";
});

// Redirecciona solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Habilita el enrutamiento a los controladores
app.MapControllers();

// Endpoint de prueba para verificar que la API funciona
app.MapGet("/test", () => "La API está funcionando correctamente!");

// Inicia la aplicación
app.Run();