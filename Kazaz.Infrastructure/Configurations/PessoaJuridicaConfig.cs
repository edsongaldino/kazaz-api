using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PessoaJuridicaConfig : IEntityTypeConfiguration<DadosPessoaJuridica>
{
    public void Configure(EntityTypeBuilder<DadosPessoaJuridica> b)
    {
        b.ToTable("dados_pessoa_juridica");

        b.Property(x => x.Cnpj).HasColumnName("cnpj").HasMaxLength(14).IsRequired();
        b.HasIndex(x => x.Cnpj).IsUnique();

        b.Property(x => x.RazaoSocial).HasColumnName("razao_social").HasMaxLength(200).IsRequired();
        b.Property(x => x.NomeFantasia).HasColumnName("nome_fantasia").HasMaxLength(200).IsRequired();
        b.Property(x => x.InscricaoEstadual).HasColumnName("inscricao_estadual").HasMaxLength(20).IsRequired();

        b.Property(x => x.DataAbertura)
            .HasColumnName("data_abertura")
            .HasColumnType("date")
            .IsRequired(false); // ou true se obrigatório

        b.HasOne(x => x.Pessoa)
            .WithOne(p => p.PessoaJuridica)
            .HasForeignKey<DadosPessoaJuridica>(x => x.Id)   // 👈 aqui!
            .OnDelete(DeleteBehavior.Cascade);
    }
}