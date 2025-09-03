using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MottuGestor.Domain.Entities;

namespace MottuGestor.Infrastructure.Mappings
{
    public class PatioMapping : IEntityTypeConfiguration<Patio>
    {
        public void Configure(EntityTypeBuilder<Patio> builder)
        {
            builder.ToTable("Patio");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Endereco)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Capacidade)
                   .IsRequired();
        }
    }
}
