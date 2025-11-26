using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContatoConfiguration : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> builder)
    {
        builder.ToTable("contatos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Tipo)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Valor)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Principal)
            .IsRequired();

        builder.Property(c => c.DataCriacao)
            .IsRequired();

        builder.HasOne(c => c.Pessoa)
            .WithMany(p => p.Contatos)
            .HasForeignKey(c => c.PessoaId);
    }
}
