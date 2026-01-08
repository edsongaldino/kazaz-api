using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Kazaz.Domain.Entities;

public class ConviteCadastroContratoConfig : IEntityTypeConfiguration<ConviteCadastroContrato>
{
    public void Configure(EntityTypeBuilder<ConviteCadastroContrato> b)
    {
        b.ToTable("convites_cadastro_contrato");

        b.HasKey(x => x.Id);

        b.Property(x => x.Token).HasColumnName("token").HasMaxLength(80).IsRequired();
        b.HasIndex(x => x.Token).IsUnique();

        b.Property(x => x.ContratoId).HasColumnName("contrato_id").IsRequired();
        b.Property(x => x.Papel).HasColumnName("papel").IsRequired();
        b.Property(x => x.Status).HasColumnName("status").IsRequired();

        b.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        b.Property(x => x.ExpiraEm).HasColumnName("expira_em");
        b.Property(x => x.UsadoEm).HasColumnName("usado_em");

        b.Property(x => x.PessoaId).HasColumnName("pessoa_id");

        b.HasOne(x => x.Contrato)
            .WithMany() // ou com navegação se quiser: Contrato.Convites
            .HasForeignKey(x => x.ContratoId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Pessoa)
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
