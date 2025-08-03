using Microsoft.EntityFrameworkCore;
using OrdenesApi;
using OrdenesApi.Models;
using Microsoft.OpenApi.Models;
using OrdenesApi.Filters;
using OrdenesApi.Repositories.Interfaces;
using OrdenesApi.Repositories;
using OrdenesApi.Services.Interfaces;
using OrdenesApi.Services;

// Punto de entrada principal de la aplicaci�n ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de CORS para permitir solicitudes desde cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configura la aplicaci�n para escuchar en puertos espec�ficos
builder.WebHost.UseUrls("http://localhost:5062", "https://localhost:7062");

// Configuraci�n de Entity Framework Core con SQL Server LocalDB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OrdenesDb;Trusted_Connection=True;"));

// Registro de controladores con filtro global de excepciones y configuraci�n JSON
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // Manejo centralizado de excepciones
})
.AddJsonOptions(options =>
{
    // Evita referencias circulares en la serializaci�n JSON
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Habilita la exploraci�n de endpoints de API
builder.Services.AddEndpointsApiExplorer();

// Configuraci�n de Swagger para documentaci�n de la API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrdenesApi",
        Version = "v1",
        Description = "API para gesti�n de �rdenes y productos"
    });
});

// Registro de repositorios en el contenedor de dependencias
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Registro de servicios en el contenedor de dependencias
builder.Services.AddScoped<IOrdenService, OrdenService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// Construcci�n de la aplicaci�n
var app = builder.Build();

// Habilita CORS con la pol�tica configurada
app.UseCors("AllowAll");

// Configuraci�n del middleware de Swagger
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
app.MapGet("/test", () => "La API est� funcionando correctamente!");

// Inicia la aplicaci�n
app.Run();