namespace FondoUnicoSistemaCompleto.Models
{
    public class RegistroEntregas
    {
        public int Id { get; set; }
        public string Unidad { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoEntrega { get; set; }
        public int cantidadActual { get; set; }
    }
}
