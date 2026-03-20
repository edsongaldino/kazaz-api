using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infra.Data.Configurations;

public class RegraDocumentoCadastroConfig : IEntityTypeConfiguration<RegraDocumentoCadastro>
{
    public void Configure(EntityTypeBuilder<RegraDocumentoCadastro> b)
    {
        b.ToTable("regras_documento_cadastro");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Enums como int (padrão do EF já é int, mas eu deixo explícito)
        b.Property(x => x.TipoPessoa)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(TipoPessoaRule.Any);

        b.Property(x => x.TipoContrato)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(TipoContratoRule.Any);

        b.Property(x => x.PapelContrato)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(PapelContratoRule.Any);

        b.Property(x => x.Obrigatorio)
            .IsRequired()
            .HasDefaultValue(true);

        b.Property(x => x.Ordem)
            .IsRequired()
            .HasDefaultValue(0);

        b.Property(x => x.Multiplicidade)
            .IsRequired()
            .HasDefaultValue(1);

        b.Property(x => x.Rotulo)
            .HasMaxLength(200);

        b.Property(x => x.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        // FK TipoDocumento
        b.HasOne(x => x.TipoDocumento)
            .WithMany() // se você tiver navigation TipoDocumento.Regras, troque aqui
            .HasForeignKey(x => x.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices úteis pro endpoint documentos-requeridos
        b.HasIndex(x => new { x.TipoPessoa, x.TipoContrato, x.PapelContrato, x.Ativo });
        b.HasIndex(x => x.TipoDocumentoId);

        // Evita duplicar regra idêntica (mesma combinação aponta pro mesmo tipo de doc)
        b.HasIndex(x => new { x.TipoDocumentoId, x.TipoPessoa, x.TipoContrato, x.PapelContrato, x.Rotulo })
            .IsUnique();

        // Check constraint: Multiplicidade >= 1
        b.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_regras_documento_cadastro_multiplicidade",
                "multiplicidade >= 1"
            );
        });
    }
}
