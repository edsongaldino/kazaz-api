using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PessoaFisicaConfig : IEntityTypeConfiguration<DadosPessoaFisica>
{
    public void Configure(EntityTypeBuilder<DadosPessoaFisica> b)
    {
        b.ToTable("dados_pessoa_fisica");              // ✅ isto é o que faltava em TPT

        b.Property(x => x.Cpf)
         .HasColumnName("cpf")
         .HasMaxLength(11)
         .IsRequired(false);

        b.HasIndex(x => x.Cpf)
         .IsUnique()
         .HasDatabaseName("ix_pessoas_cpf_1");

        b.Property(p => p.Rg)
           .HasColumnName("rg")
           .HasMaxLength(20);       // exemplo

        b.Property(p => p.OrgaoExpedidor)
               .HasColumnName("orgao_expedidor")
               .HasMaxLength(50);

        b.Property(x => x.DataNascimento)
         .HasColumnName("data_nascimento")
         .HasColumnType("date");
    }
}
