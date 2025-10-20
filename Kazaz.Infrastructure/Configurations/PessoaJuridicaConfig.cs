using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PessoaJuridicaConfig : IEntityTypeConfiguration<DadosPessoaJuridica>
{
    public void Configure(EntityTypeBuilder<DadosPessoaJuridica> b)
    {
        b.ToTable("dados_pessoa_juridica");           // ✅ idem

        b.Property(x => x.Cnpj).HasMaxLength(14).IsRequired();
        b.Property(x => x.RazaoSocial).HasMaxLength(200).IsRequired();
        b.HasIndex(x => x.Cnpj).IsUnique();
    }
}
