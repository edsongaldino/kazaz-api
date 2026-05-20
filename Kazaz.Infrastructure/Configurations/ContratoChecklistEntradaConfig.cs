using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContratoChecklistEntradaConfig : IEntityTypeConfiguration<ContratoChecklistEntrada>
{
    public void Configure(EntityTypeBuilder<ContratoChecklistEntrada> b)
    {
        b.ToTable("contratos_checklist_entrada");
        b.HasKey(x => x.ContratoId);

        b.Property(x => x.ContratoId).HasColumnName("contrato_id");
        b.Property(x => x.AssinadoEm).HasColumnName("assinado_em");
        b.Property(x => x.SeguroIncendio).HasColumnName("seguro_incendio").HasMaxLength(250);
        b.Property(x => x.Chaves).HasColumnName("chaves").HasMaxLength(250);
        b.Property(x => x.Energia).HasColumnName("energia").HasMaxLength(250);
        b.Property(x => x.Agua).HasColumnName("agua").HasMaxLength(250);
        b.Property(x => x.Gas).HasColumnName("gas").HasMaxLength(250);
        b.Property(x => x.Condominio).HasColumnName("condominio").HasMaxLength(250);
        b.Property(x => x.IptuGaragem).HasColumnName("iptu_garagem").HasMaxLength(250);
        b.Property(x => x.Iptu).HasColumnName("iptu").HasMaxLength(250);
        b.Property(x => x.VistoriaEntradaEm).HasColumnName("vistoria_entrada_em");
        b.Property(x => x.Manutencao).HasColumnName("manutencao").HasMaxLength(1000);
        b.Property(x => x.ObservacoesFinais).HasColumnName("observacoes_finais").HasMaxLength(1000);
        b.Property(x => x.BonusLocacao).HasColumnName("bonus_locacao").HasMaxLength(250);
        b.Property(x => x.DataPagamentoBonus).HasColumnName("data_pagamento_bonus");

        b.HasOne(x => x.Contrato)
            .WithOne()
            .HasForeignKey<ContratoChecklistEntrada>(x => x.ContratoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
