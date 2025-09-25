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
            
            optionsBuilder.UseOracle("User ID=rm556426;Password=300103;Data Source=oracle.fiap.com.br:1521/orcl;");

            return new GestMottuContext(optionsBuilder.Options);
        }
    }
}