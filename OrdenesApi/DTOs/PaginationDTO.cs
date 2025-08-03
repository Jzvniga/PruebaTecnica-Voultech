using Microsoft.AspNetCore.Mvc;
namespace OrdenesApi.DTOs
{
    /// <summary>
    /// DTO gen�rico para respuestas paginadas.
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
    /// Par�metros para solicitudes de paginaci�n con l�mite m�ximo de 50 elementos por p�gina.
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