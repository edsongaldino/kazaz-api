using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public class PerfilVinculoImovelConfig : IEntityTypeConfiguration<PerfilVinculoImovel>
{
    public void Configure(EntityTypeBuilder<PerfilVinculoImovel> b)
    {
        b.ToTable("perfis_vinculo_imovel");
        b.HasKey(x => x.Id);

        b.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(100);

        b.HasIndex(x => x.Nome).IsUnique();
    }
}
