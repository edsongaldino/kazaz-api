using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public class FinanceiroLancamentoConfiguration : IEntityTypeConfiguration<FinanceiroLancamento>
{
    public void Configure(EntityTypeBuilder<FinanceiroLancamento> b)
    {
        b.ToTable("financeiro_lancamentos");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Valor)
            .HasColumnName("valor")
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.DataVencimento)
            .HasColumnName("data_vencimento")
            .IsRequired();

        b.Property(x => x.DataPagamento)
            .HasColumnName("data_pagamento");

        b.Property(x => x.Categoria)
            .HasColumnName("categoria")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.ClienteId)
            .HasColumnName("cliente_id");

        b.Property(x => x.ContratoId)
            .HasColumnName("contrato_id");

        b.HasOne(x => x.Cliente)
            .WithMany()
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Contrato)
            .WithMany()
            .HasForeignKey(x => x.ContratoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Property(x => x.ImobiliariaId)
            .HasColumnName("imobiliaria_id");

        b.HasOne(x => x.Imobiliaria)
            .WithMany()
            .HasForeignKey(x => x.ImobiliariaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Tipo).HasDatabaseName("ix_financeiro_lancamentos_tipo");
        b.HasIndex(x => x.Status).HasDatabaseName("ix_financeiro_lancamentos_status");
        b.HasIndex(x => x.DataVencimento).HasDatabaseName("ix_financeiro_lancamentos_data_vencimento");
        b.HasIndex(x => x.ClienteId).HasDatabaseName("ix_financeiro_lancamentos_cliente_id");
        b.HasIndex(x => x.ContratoId).HasDatabaseName("ix_financeiro_lancamentos_contrato_id");
    }
}
