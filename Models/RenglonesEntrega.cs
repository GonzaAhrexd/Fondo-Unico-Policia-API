namespace SistemaFondoUnicoAPI.Models
{
    public class RenglonesEntrega
    {
        public int Id { get; set; }
        public int NroRenglon { get; set; }
        public String TipoFormulario { get; set; }
        public int desde { get; set; }
        public int hasta { get; set; }
        public int cantidad { get; set; }

    }
}
