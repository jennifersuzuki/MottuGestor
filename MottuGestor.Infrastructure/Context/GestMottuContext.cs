using Microsoft.EntityFrameworkCore;
using MottuGestor.Domain.Entities;
using MottuGestor.Infrastructure.Mappings;

namespace MottuGestor.Infrastructure.Context
{
    public class GestMottuContext : DbContext
    {
        public GestMottuContext(DbContextOptions<GestMottuContext> options) : base(options)
        {
        }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Patio> Patios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; } // ✅ Adicionado

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MotoMapping());
            modelBuilder.ApplyConfiguration(new PatioMapping());
            // modelBuilder.ApplyConfiguration(new UsuarioMapping()); // ✅ Adicionado

            base.OnModelCreating(modelBuilder);
        }
    }
}
