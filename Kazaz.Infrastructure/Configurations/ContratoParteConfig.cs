using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContratoParteConfig : IEntityTypeConfiguration<ContratoParte>
{
    public void Configure(EntityTypeBuilder<ContratoParte> b)
    {
        b.ToTable("contrato_partes");
        b.HasKey(x => x.Id);

        b.Property(x => x.ContratoId).HasColumnName("contrato_id");
        b.Property(x => x.PessoaId).HasColumnName("pessoa_id");
        b.Property(x => x.Papel).HasColumnName("papel").HasConversion<int>();

        b.Property(x => x.Percentual).HasColumnName("percentual");

        b.HasOne(x => x.Pessoa)
            .WithMany(p => p.Contratos)
            .HasForeignKey(x => x.PessoaId);

        b.HasIndex(x => new { x.ContratoId, x.PessoaId, x.Papel })
            .IsUnique(); // evita duplicar a mesma pessoa no mesmo papel no mesmo contrato
    }
}
