using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PessoaConfig : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> b)
    {
        b.ToTable("pessoas");
        b.HasKey(x => x.Id);

        b.HasOne(x => x.Endereco)
         .WithMany()
         .HasForeignKey(x => x.EnderecoId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(p => p.PessoaFisica)
         .WithOne(pf => pf.Pessoa)
         .HasForeignKey<DadosPessoaFisica>(pf => pf.PessoaId)   // shared PK
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(p => p.PessoaJuridica)
         .WithOne(pj => pj.Pessoa)
         .HasForeignKey<DadosPessoaJuridica>(pj => pj.PessoaId) // shared PK
         .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.OrigemId)
        .HasColumnName("origem_id");

            b.HasOne(x => x.Origem)
                .WithMany(o => o.Clientes)
                .HasForeignKey(x => x.OrigemId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
