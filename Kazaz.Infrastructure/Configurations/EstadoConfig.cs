using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public sealed class EstadoConfig : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> b)
    {
        b.ToTable("estados");
        b.HasKey(x => x.Id);

        b.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(120);

        b.Property(x => x.Uf)
            .IsRequired()
            .HasMaxLength(2);

        // UF único (ex.: MT, SP, RJ...)
        b.HasIndex(x => x.Uf).IsUnique();

        // Estado 1:N Cidades
        b.HasMany(x => x.Cidades)
            .WithOne(x => x.Estado)
            .HasForeignKey(x => x.EstadoId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
