using Microsoft.Identity.Client;

namespace FondoUnicoSistemaCompleto.Models
{
    public class Depositos
    {
        public int Id { get; set; }
        public int NroDeposito { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime PeriodoArqueo { get; set; }
        public string Unidad { get; set; }
        public string Banco { get; set; }
        public string Cuenta { get; set; }
        public int Boleta { get; set; }
        public float Importe { get; set; }

    }
}
