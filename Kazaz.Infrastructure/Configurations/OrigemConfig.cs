using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Infrastructure.Configurations;

public class OrigemConfiguration : IEntityTypeConfiguration<Origem>
{
    public void Configure(EntityTypeBuilder<Origem> b)
    {
        b.ToTable("origens");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(120)
            .IsRequired();

        b.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(500);

        // Índice para buscas por nome
        b.HasIndex(x => x.Nome).HasDatabaseName("ix_origens_nome");
    }
}