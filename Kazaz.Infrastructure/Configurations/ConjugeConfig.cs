using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;
public class ConjugeConfig : IEntityTypeConfiguration<Conjuge>
{
    public void Configure(EntityTypeBuilder<Conjuge> builder)
    {
        builder.ToTable("conjuges");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Cpf)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(c => c.Rg)
            .HasMaxLength(20);

        builder.Property(c => c.OrgaoExpedidor)
            .HasMaxLength(20);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        builder.Property(c => c.Email)
            .HasMaxLength(150);

        builder.Property(c => c.DataCriacao)
            .IsRequired();

        builder.HasOne(c => c.Pessoa)
            .WithOne(p => p.Conjuge)
            .HasForeignKey<Conjuge>(c => c.PessoaId);
    }
}
