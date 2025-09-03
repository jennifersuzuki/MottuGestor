// using MottuGestor.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using MottuGestor.Domain.Entities;
//
// namespace MottuGestor.Infrastructure.Mappings
// {
//     public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
//     {
//         public void Configure(EntityTypeBuilder<Usuario> builder)
//         {
//             builder.ToTable("Usuario");
//
//             builder.HasKey(u => u.UsuarioId);
//
//             builder.Property(u => u.Nome)
//                 .IsRequired()
//                 .HasMaxLength(100);
//
//             builder.Property(u => u.Email)
//                 .IsRequired()
//                 .HasMaxLength(150);
//
//             builder.Property(u => u.SenhaHash)
//                 .IsRequired();
//
//             builder.Property(u => u.DataCadastro)
//                 .IsRequired();
//         }
//     }
// }
