using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContratoChecklistSaidaConfig : IEntityTypeConfiguration<ContratoChecklistSaida>
{
    public void Configure(EntityTypeBuilder<ContratoChecklistSaida> b)
    {
        b.ToTable("contratos_checklist_saida");
        b.HasKey(x => x.ContratoId);

        b.Property(x => x.ContratoId).HasColumnName("contrato_id");
        b.Property(x => x.MotivoSaida).HasColumnName("motivo_saida").HasMaxLength(500);
        b.Property(x => x.Aluguel).HasColumnName("aluguel").HasMaxLength(250);
        b.Property(x => x.MultaContratual).HasColumnName("multa_contratual").HasMaxLength(250);
        b.Property(x => x.AvisoSaidaEm).HasColumnName("aviso_saida_em");
        b.Property(x => x.Chaves).HasColumnName("chaves").HasMaxLength(250);
        b.Property(x => x.AvisoProprietario).HasColumnName("aviso_proprietario").HasMaxLength(250);
        b.Property(x => x.Energia).HasColumnName("energia").HasMaxLength(250);
        b.Property(x => x.Gas).HasColumnName("gas").HasMaxLength(250);
        b.Property(x => x.Agua).HasColumnName("agua").HasMaxLength(250);
        b.Property(x => x.Condominio).HasColumnName("condominio").HasMaxLength(250);
        b.Property(x => x.Iptu).HasColumnName("iptu").HasMaxLength(250);
        b.Property(x => x.VistoriaSaidaEm).HasColumnName("vistoria_saida_em");
        b.Property(x => x.PinturaManutencao).HasColumnName("pintura_manutencao").HasMaxLength(1000);
        b.Property(x => x.ReativarImovelNoSite).HasColumnName("reativar_imovel_no_site").HasMaxLength(250);
        b.Property(x => x.CancelamentoSeguroFianca).HasColumnName("cancelamento_seguro_fianca").HasMaxLength(250);

        b.HasOne(x => x.Contrato)
            .WithOne()
            .HasForeignKey<ContratoChecklistSaida>(x => x.ContratoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
