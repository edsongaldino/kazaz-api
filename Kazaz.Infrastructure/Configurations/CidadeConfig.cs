using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public sealed class CidadeConfig : IEntityTypeConfiguration<Cidade>
{
    public void Configure(EntityTypeBuilder<Cidade> b)
    {
        b.ToTable("cidades");
        b.HasKey(x => x.Id);

        b.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(120);

        b.Property(x => x.Ibge)
            .HasMaxLength(10); // ajuste se usar outro formato

        b.Property(x => x.EstadoId)
            .IsRequired();

        // Nome único por Estado
        b.HasIndex(x => new { x.EstadoId, x.Nome }).IsUnique();

        // Código IBGE pode ser único (se você quiser garantir isso globalmente)
        b.HasIndex(x => x.Ibge).IsUnique(false); // mude para true se quiser único

        // Cidade N:1 Estado
        b.HasOne(x => x.Estado)
            .WithMany(x => x.Cidades)
            .HasForeignKey(x => x.EstadoId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

    }
}
