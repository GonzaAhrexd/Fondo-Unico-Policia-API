using FondoUnicoSistemaCompleto.Models;
using Microsoft.EntityFrameworkCore;
using SistemaFondoUnicoAPI.Models;

namespace FondoUnicoSistemaCompleto.Context
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
        {}

    // Define your DbSets here
 
        public DbSet<FondoUnicoSistemaCompleto.Models.Usuario> Usuario { get; set; } = default!;
 
        public DbSet<FondoUnicoSistemaCompleto.Models.Bancos> Bancos { get; set; } = default!;
 
        public DbSet<SistemaFondoUnicoAPI.Models.Formularios> Formularios { get; set; } = default!;
 
        public DbSet<FondoUnicoSistemaCompleto.Models.Localidades> Localidades { get; set; } = default!;
 
        public DbSet<FondoUnicoSistemaCompleto.Models.Unidades> Unidades { get; set; } = default!;
        public DbSet<RenglonesEntrega> RenglonesEntregas { get; set; }
        public DbSet<Entregas> Entregas { get; set; }
    }
}