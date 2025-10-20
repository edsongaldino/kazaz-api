using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Infrastructure.Configurations;

public sealed class TipoDocumentoConfig : IEntityTypeConfiguration<TipoDocumento>
{
    public void Configure(EntityTypeBuilder<TipoDocumento> b)
    {
        b.ToTable("tipos_documento");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).IsRequired().HasMaxLength(150);
        b.Property(x => x.Alvo).IsRequired();
        b.HasIndex(x => new { x.Alvo, x.Nome }).IsUnique(); // evita duplicar mesmo tipo pro mesmo alvo
    }
}
