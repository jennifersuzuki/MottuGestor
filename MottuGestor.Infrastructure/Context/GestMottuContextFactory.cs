using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace MottuGestor.Infrastructure.Context
{
    public class GestMottuContextFactory : IDesignTimeDbContextFactory<GestMottuContext>
    {
        public GestMottuContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GestMottuContext>();

            // Aqui você coloca sua connection string de desenvolvimento
            optionsBuilder.UseSqlServer("Server=localhost;Database=MottuGestorDb;Trusted_Connection=True;");

            return new GestMottuContext(optionsBuilder.Options);
        }
    }
}