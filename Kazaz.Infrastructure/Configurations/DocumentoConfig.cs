using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Infrastructure.Configurations;

public sealed class DocumentoConfig : IEntityTypeConfiguration<Documento>
{
    public void Configure(EntityTypeBuilder<Documento> b)
    {
        b.ToTable("documentos");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).IsRequired().HasMaxLength(200);
        b.Property(x => x.Caminho).IsRequired().HasMaxLength(500);
        b.Property(x => x.ContentType).HasMaxLength(120);
    }
}
