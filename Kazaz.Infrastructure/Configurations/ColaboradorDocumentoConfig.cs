using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public sealed class ColaboradorDocumentoConfig : IEntityTypeConfiguration<ColaboradorDocumento>
{
    public void Configure(EntityTypeBuilder<ColaboradorDocumento> b)
    {
        b.ToTable("colaboradores_documentos");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.ColaboradorId).HasColumnName("colaborador_id").IsRequired();
        b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
        b.Property(x => x.DocumentoId).HasColumnName("documento_id").IsRequired();
        b.Property(x => x.DataAnexo).HasColumnName("data_anexo").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();

        // Relationships
        b.HasOne(x => x.Colaborador)
            .WithMany(c => c.Documentos)
            .HasForeignKey(x => x.ColaboradorId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Documento)
            .WithMany()
            .HasForeignKey(x => x.DocumentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
