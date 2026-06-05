using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public class PrestadorServicoConfiguration : IEntityTypeConfiguration<PrestadorServico>
{
    public void Configure(EntityTypeBuilder<PrestadorServico> b)
    {
        b.ToTable("prestadores_servicos");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Especialidade)
            .HasColumnName("especialidade")
            .HasConversion<string>()
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.CpfCnpj)
            .HasColumnName("cpf_cnpj")
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.Telefone)
            .HasColumnName("telefone")
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(150);

        b.Property(x => x.Ativo)
            .HasColumnName("ativo")
            .HasDefaultValue(true)
            .IsRequired();

        b.Property(x => x.Observacoes)
            .HasColumnName("observacoes")
            .HasMaxLength(1000);

        b.Property(x => x.EnderecoId)
            .HasColumnName("endereco_id");

        b.HasOne(x => x.Endereco)
            .WithMany()
            .HasForeignKey(x => x.EnderecoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Property(x => x.ImobiliariaId)
            .HasColumnName("imobiliaria_id");

        b.HasOne(x => x.Imobiliaria)
            .WithMany()
            .HasForeignKey(x => x.ImobiliariaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Especialidade).HasDatabaseName("ix_prestadores_servicos_especialidade");
        b.HasIndex(x => x.EnderecoId).HasDatabaseName("ix_prestadores_servicos_endereco_id");
    }
}
