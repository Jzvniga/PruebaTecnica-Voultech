# API de Gestión de Órdenes y Productos

Una API REST desarrollada en .NET 7 para administrar órdenes de compra y productos, implementando un sistema completo de CRUD con características avanzadas como paginación y un algoritmo personalizado de descuentos.

---

## Características Principales

- ✅ API RESTful completa con endpoints para gestión de órdenes y productos
- ✅ Arquitectura en capas (Controllers, Services, Repositories)
- ✅ Algoritmo de descuentos dinámico:
  - 10% de descuento si el total > $500
  - 5% adicional si hay más de 5 productos distintos
- ✅ Documentación automática con Swagger/OpenAPI
- ✅ Paginación en el listado de órdenes
- ✅ Manejo global de excepciones
- ✅ Pruebas unitarias con xUnit y Moq

---

## Tecnologías Utilizadas

- .NET 7
- C#
- Entity Framework Core
- SQL Server LocalDB
- Swagger/OpenAPI
- xUnit + Moq

---

##  Requisitos Previos

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server LocalDB](https://learn.microsoft.com/es-es/sql/database-engine/configure-windows/sql-server-express-localdb)
- Visual Studio 2022

---

##  Instalación y Ejecución

1. **Clonar el repositorio**

```bash
git clone <url-del-repositorio>
cd OrdenesApi
```

2. **Restaurar dependencias**

```bash
dotnet restore
```

3. **Configurar la base de datos**

La app usa SQL Server LocalDB con la siguiente cadena de conexión:

```
Server=(localdb)\MSSQLLocalDB;Database=OrdenesDb;Trusted_Connection=True;
```

La base de datos se crea automáticamente al ejecutar la aplicación.

4. **Ejecutar la aplicación**

```bash
dotnet run --project OrdenesApi
```

La API estará disponible en:

- http://localhost:5062  
- https://localhost:7062

5. **Acceder a la documentación Swagger**

```url
http://localhost:5062/swagger
```

---

## Configuración de Base de Datos

El proyecto utiliza Entity Framework Core con un `ApplicationDbContext` para mapear las entidades a tablas de la base de datos. Las migraciones se gestionan automáticamente al iniciar la aplicación.

---

## Estructura del Proyecto

```
OrdenesApi/
│
├── Controllers/
│   ├── OrdenesController.cs
│   ├── ProductosController.cs
│   └── TestExceptionController.cs
│
├── Services/
│   ├── OrdenService.cs
│   └── ProductoService.cs
│
├── Repositories/
│   ├── Interfaces/
│   └── Implementaciones/
│
├── Models/
│   ├── Orden.cs
│   ├── Producto.cs
│   └── OrdenProducto.cs
│
├── DTOs/
├── Filters/
├── ApplicationDbContext.cs
└── Program.cs
```

---

## Estructura de DTOs

- **OrdenDTO**: DTO para creación y actualización de órdenes
- **PaginationDTO**: Facilita la paginación de resultados
- **ProductoDTO**: DTO para transferencia de datos de productos
- **ProductoResponseDTO**: DTO específico para respuestas de productos

---

## Endpoints de la API

### Productos

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/productos`        | Obtener todos los productos         |
| GET    | `/api/productos/{id}`   | Obtener un producto por ID          |
| POST   | `/api/productos`        | Crear un nuevo producto             |
| PUT    | `/api/productos/{id}`   | Actualizar un producto existente    |
| DELETE | `/api/productos/{id}`   | Eliminar un producto                |

### Órdenes

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/ordenes`         | Obtener órdenes paginadas           |
| GET    | `/api/ordenes/{id}`    | Obtener una orden por ID            |
| POST   | `/api/ordenes`         | Crear una nueva orden               |
| PUT    | `/api/ordenes/{id}`    | Actualizar una orden existente      |
| DELETE | `/api/ordenes/{id}`    | Eliminar una orden y sus relaciones |

---

## Ejemplos de Uso

### Crear un Producto

**Request:**

```http
POST /api/productos
Content-Type: application/json

{
  "nombre": "Laptop XPS",
  "precio": 1200.00
}
```

**Response:**

```json
{
  "id": 1,
  "nombre": "Laptop XPS",
  "precio": 1200.00
}
```

---

### Crear una Orden

**Request:**

```http
POST /api/ordenes
Content-Type: application/json

{
  "cliente": "Juan Pérez",
  "productosIds": [1, 2, 3]
}
```

**Response:**

```json
{
  "id": 1,
  "cliente": "Juan Pérez",
  "total": 1140.00,
  "productos": [
    { "id": 1, "nombre": "Laptop XPS", "precio": 1200.00 },
    { "id": 2, "nombre": "Mouse Inalámbrico", "precio": 50.00 },
    { "id": 3, "nombre": "Teclado Mecánico", "precio": 100.00 }
  ]
}
```

---

### Obtener Órdenes Paginadas

```http
GET /api/ordenes?NumeroPagina=1&ElementosPorPagina=10
```

**Response:**

```json
{
  "totalItems": 50,
  "totalPages": 5,
  "currentPage": 1,
  "pageSize": 10,
  "ordenes": [
    {
      "id": 1,
      "cliente": "Juan Pérez",
      "total": 1140.00,
      "productos": [ ... ]
    },
    {
      "id": 2,
      "cliente": "Ana Gómez",
      "total": 980.00,
      "productos": [ ... ]
    }
  ]
}

```
Fotos en postman:

Pedir todos los productos
<img width="1440" height="823" alt="image" src="https://github.com/user-attachments/assets/8414fa10-810d-4bdf-85e4-541016b29245" />

Crear una orden
<img width="1422" height="824" alt="image" src="https://github.com/user-attachments/assets/7754e766-9a55-4a15-857d-687560e38cbc" />

Actualizar Orden 
<img width="1378" height="397" alt="image" src="https://github.com/user-attachments/assets/c59077c9-ab17-4113-9409-62133879f860" />
<img width="1387" height="621" alt="image" src="https://github.com/user-attachments/assets/cedcc930-c6bd-41da-99f9-ae014d146ea7" />


## 🧪 Pruebas Unitarias

Se incluyen pruebas unitarias enfocadas en:

- Lógica del algoritmo de descuentos
- Operaciones CRUD de productos y órdenes

**Para ejecutarlas:**

```bash
cd OrdenesApi.Tests
dotnet test
```

## Decisiones Técnicas

- ✅ **Patrón Repositorio**: para separar la lógica de negocio del acceso a datos
- ✅ **Inyección de Dependencias**: facilita pruebas y reduce acoplamiento
- ✅ **DTOs**: para proteger y controlar los datos transferidos
- ✅ **Filtro Global de Excepciones**: respuestas de error uniformes
- ✅ **Cálculo de Descuentos**: encapsulado en la capa de servicios
- ✅ **Interfaces para Servicios**: Implementé interfaces para todos los servicios, siguiendo el principio de segregación de interfaces de SOLID

---

## Posibles Mejoras Futuras

- Actualización a .NET 8 (versión LTS)
- Implementación de autenticación y autorización
- Expansión de las pruebas unitarias
- Implementación de caché para mejorar rendimiento

---

## Contacto

- LinkedIn: [Jose Zuniga](https://www.linkedin.com/in/jose-zuniga-o/)
- GitHub: [Jzvniga](https://github.com/Jzvniga)
- Email: [jzunigadevelopment@gmail.com]

