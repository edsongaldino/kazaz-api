using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Data.Configurations;

public class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
{
    public void Configure(EntityTypeBuilder<Perfil> b)
    {
        b.ToTable("perfis");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id");

        b.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(120)
            .IsRequired();

        b.HasIndex(x => x.Nome).IsUnique(); // se no seu SQL já for unique, ótimo
    }
}
