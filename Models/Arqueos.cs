namespace FondoUnicoSistemaCompleto.Models
{
    public class Arqueos
    {
        public int Id { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string? Unidad { get; set; }
        public string? TipoDeFormulario { get; set; }
        // Valores entregados
        public int TotalEntregado { get; set; }
        public float ValorEntregado { get; set; }
        // Saldo valores arqueo anterior
        public int ArqueoAnteriorCantidad { get; set; }
        public float ArqueoAnteriorImporte { get; set; }
        // Existencia actual
        public int CantidadRestante { get; set; }
        public float TotalSobrante { get; set; }
        // Depósitos efectuados
        public int cantidadDepositos { get; set; }
        public float TotalDepositos { get; set; }
        public float CantidadADepositar { get; set; }
        public int TotalEntregadoImporte { get; set; }
        public bool Coincidente { get; set; } = true;
    }
}
