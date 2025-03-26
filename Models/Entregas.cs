using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaFondoUnicoAPI.Models
{
    public class Entregas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Configura como auto incremental
        public int NroEntrega { get; set; }
        public int NroEntregaManual { get; set; }
        public DateTime Fecha { get; set; }
        public string Unidad { get; set; }
        public bool estaActivo { get; set; }
        public virtual List<RenglonesEntrega> RenglonesEntregas { get; set; }


    }
}
