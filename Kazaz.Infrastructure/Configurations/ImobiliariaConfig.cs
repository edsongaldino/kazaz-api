using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public class ImobiliariaConfiguration : IEntityTypeConfiguration<Imobiliaria>
{
    public void Configure(EntityTypeBuilder<Imobiliaria> b)
    {
        b.ToTable("imobiliarias");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.RazaoSocial)
            .HasColumnName("razao_social")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.NomeFantasia)
            .HasColumnName("nome_fantasia")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Cnpj)
            .HasColumnName("cnpj")
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.Creci)
            .HasColumnName("creci")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.DataFundacao)
            .HasColumnName("data_fundacao");

        b.Property(x => x.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500);

        b.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(150);

        b.Property(x => x.Telefone)
            .HasColumnName("telefone")
            .HasMaxLength(50);

        b.Property(x => x.EnderecoId)
            .HasColumnName("endereco_id");

        b.HasOne(x => x.Endereco)
            .WithMany()
            .HasForeignKey(x => x.EnderecoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Cnpj).IsUnique().HasDatabaseName("ix_imobiliarias_cnpj");
    }
}
