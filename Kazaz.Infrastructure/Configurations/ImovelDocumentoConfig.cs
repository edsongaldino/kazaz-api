using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Infrastructure.Configurations;

public sealed class ImovelDocumentoConfig : IEntityTypeConfiguration<ImovelDocumento>
{
    public void Configure(EntityTypeBuilder<ImovelDocumento> b)
    {
        b.ToTable("imoveis_documentos");
        b.HasKey(x => x.Id);

        b.HasOne(x => x.Imovel).WithMany()
            .HasForeignKey(x => x.ImovelId).OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Tipo).WithMany()
            .HasForeignKey(x => x.TipoDocumentoId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Documento).WithMany()
            .HasForeignKey(x => x.DocumentoId).OnDelete(DeleteBehavior.Cascade);

        // Imóvel não pode ter duas vezes o MESMO tipo
        b.HasIndex(x => new { x.ImovelId, x.TipoDocumentoId }).IsUnique()
         .HasDatabaseName("UX_imoveis_documentos_unique");
    }
}