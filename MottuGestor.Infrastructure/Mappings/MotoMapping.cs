using MottuGestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MottuGestor.Infrastructure.Mappings
{
    public class MotoMapping : IEntityTypeConfiguration<Moto>
    {
        public void Configure(EntityTypeBuilder<Moto> builder)
        {
            builder.ToTable("Moto");

            builder.HasKey(m => m.MotoId);

            builder.Property(m => m.Placa)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(m => m.Modelo)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(m => m.Marca)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.RfidTag)
                .IsRequired();

            builder.Property(m => m.Ano)
                .IsRequired();

            builder.Property(m => m.DataCadastro)
                .IsRequired();

            builder.Property(m => m.Problema)
                .HasMaxLength(200);

            // builder.Property(m => m.Localizacao)
            //     .HasMaxLength(100);

            builder.Property(m => m.Status)
                .IsRequired();
        }
    }
}
