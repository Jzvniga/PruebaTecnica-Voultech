using Microsoft.AspNetCore.Mvc;
namespace OrdenesApi.DTOs
{
    /// <summary>
    /// DTO genérico para respuestas paginadas.
    /// </summary>
    public class PaginationDTO<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PaginaActual { get; set; }
        public int ElementosPorPagina { get; set; }
        public int TotalElementos { get; set; }
        public int TotalPaginas { get; set; }
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    }

    /// <summary>
    /// Parámetros para solicitudes de paginación con límite máximo de 50 elementos por página.
    /// </summary>
    public class PaginationParams
    {
        private const int MaximoPaginaSize = 50;
        private int _elementosPorPagina = 10;
        
        [FromQuery(Name = "numeroPagina")]
        public int NumeroPagina { get; set; } = 1;
        
        [FromQuery(Name = "elementosPorPagina")]
        public int ElementosPorPagina
        {
            get => _elementosPorPagina;
            set => _elementosPorPagina = value > MaximoPaginaSize ? MaximoPaginaSize : value;
        }
    }
}