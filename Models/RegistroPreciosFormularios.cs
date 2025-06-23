namespace FondoUnicoSistemaCompleto.Models
{
    public class RegistroPreciosFormularios
    {
        public int Id { get; set; }
        public DateTime desdeActivo { get; set; }
        public DateTime? hastaActivo { get; set; }

        public string Formulario { get; set; }
        public float Importe { get; set; }
    }
}
