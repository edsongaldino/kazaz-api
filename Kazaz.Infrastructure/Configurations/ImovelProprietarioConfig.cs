using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public sealed class ImovelProprietarioConfig : IEntityTypeConfiguration<ImovelProprietario>
{
    public void Configure(EntityTypeBuilder<ImovelProprietario> b)
    {
        b.ToTable("imovel_proprietarios");
        b.HasKey(x => x.Id);

        b.Property(x => x.ImovelId).HasColumnName("imovel_id").IsRequired();
        b.Property(x => x.PessoaId).HasColumnName("pessoa_id").IsRequired();
        b.Property(x => x.Percentual).HasColumnName("percentual").HasPrecision(5, 2);
        b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true).IsRequired();
        b.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();

        b.HasOne(x => x.Imovel)
            .WithMany(i => i.Proprietarios)
            .HasForeignKey(x => x.ImovelId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Pessoa)
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
