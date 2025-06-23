namespace FondoUnicoSistemaCompleto.Models
{
    public class Arqueos
    {
        public int Id { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }

        public string? Unidad { get; set; }
        public string? TipoDeFormulario { get; set; }
        public int CantidadUtilizada { get; set; }
        public int TotalEntregado { get; set; }
        public int TotalSobrante { get; set; }
        public float TotalDepositos { get; set; }
        public float Valor { get; set; }

    }
}
