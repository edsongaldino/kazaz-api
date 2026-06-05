using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> b)
    {
        b.ToTable("leads");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(150);

        b.Property(x => x.Telefone)
            .HasColumnName("telefone")
            .HasMaxLength(50);

        b.Property(x => x.OrigemId)
            .HasColumnName("origem_id");

        b.Property(x => x.ImovelId)
            .HasColumnName("imovel_id");

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Mensagem)
            .HasColumnName("mensagem")
            .HasMaxLength(2000);

        b.Property(x => x.PessoaId)
            .HasColumnName("pessoa_id");

        b.Property(x => x.DataCriacao)
            .HasColumnName("data_criacao")
            .IsRequired();

        b.Property(x => x.DataAtualizacao)
            .HasColumnName("data_atualizacao");

        // Relacionamentos e índices
        b.HasOne(x => x.Origem)
            .WithMany()
            .HasForeignKey(x => x.OrigemId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Imovel)
            .WithMany()
            .HasForeignKey(x => x.ImovelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Pessoa)
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Property(x => x.ImobiliariaId)
            .HasColumnName("imobiliaria_id");

        b.HasOne(x => x.Imobiliaria)
            .WithMany()
            .HasForeignKey(x => x.ImobiliariaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Status).HasDatabaseName("ix_leads_status");
        b.HasIndex(x => x.OrigemId).HasDatabaseName("ix_leads_origem_id");
        b.HasIndex(x => x.PessoaId).HasDatabaseName("ix_leads_pessoa_id");
    }
}
