using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PessoaConfig : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> b)
    {
        b.ToTable("pessoas");
        b.HasKey(x => x.Id);

        b.Property(x => x.Nome).IsRequired().HasMaxLength(200);

        b.HasOne(x => x.Endereco)
         .WithMany()
         .HasForeignKey(x => x.EnderecoId)
         .OnDelete(DeleteBehavior.SetNull);

        // ❌ NADA de HasDiscriminator aqui (isso força TPH)
    }
}
