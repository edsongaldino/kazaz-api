using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Kazaz.Infrastructure.Configurations;


public sealed class FotoConfig : IEntityTypeConfiguration<Foto>
{
    public void Configure(EntityTypeBuilder<Foto> b)
    {
        b.ToTable("fotos");
        b.HasKey(x => x.Id);
        b.Property(x => x.Caminho).IsRequired().HasMaxLength(400);
        b.HasIndex(x => new { x.ImovelId, x.Ordem }).IsUnique();
    }
}