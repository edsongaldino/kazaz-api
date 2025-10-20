using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Infrastructure.Configurations;

public sealed class PessoaDocumentoConfig : IEntityTypeConfiguration<PessoaDocumento>
{
    public void Configure(EntityTypeBuilder<PessoaDocumento> b)
    {
        b.ToTable("pessoas_documentos");
        b.HasKey(x => x.Id);

        b.HasOne(x => x.Pessoa).WithMany()  // se quiser: WithMany(p => p.DocumentosPessoa)
            .HasForeignKey(x => x.PessoaId).OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Tipo).WithMany()
            .HasForeignKey(x => x.TipoDocumentoId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Documento).WithMany()
            .HasForeignKey(x => x.DocumentoId).OnDelete(DeleteBehavior.Cascade);

        // Pessoa não pode ter duas vezes o MESMO tipo
        b.HasIndex(x => new { x.PessoaId, x.TipoDocumentoId }).IsUnique()
         .HasDatabaseName("UX_pessoas_documentos_unique");
    }
}
