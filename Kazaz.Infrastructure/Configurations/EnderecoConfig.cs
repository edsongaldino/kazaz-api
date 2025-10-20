using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public sealed class EnderecoConfig : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> b)
    {
        b.ToTable("enderecos");
        b.HasKey(x => x.Id);

        b.Property(x => x.Cep).HasMaxLength(9); // "00000-000" | use 8 se salvar sem máscara
        b.Property(x => x.Logradouro).IsRequired().HasMaxLength(200);
        b.Property(x => x.Numero).IsRequired().HasMaxLength(20);
        b.Property(x => x.Complemento).HasMaxLength(120);
        b.Property(x => x.Bairro).IsRequired().HasMaxLength(120);

        b.HasOne(x => x.Cidade)
         .WithMany() // 👈 não referencia Cidade.Enderecos
         .HasForeignKey(x => x.CidadeId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);

        // Exemplos de índices úteis
        b.HasIndex(x => x.Cep);
        b.HasIndex(x => new { x.Logradouro, x.Numero, x.Bairro, x.CidadeId });
    }
}
