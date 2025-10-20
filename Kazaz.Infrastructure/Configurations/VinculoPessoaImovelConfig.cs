using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Kazaz.Infrastructure.Configurations;


public sealed class VinculoPessoaImovelConfig : IEntityTypeConfiguration<VinculoPessoaImovel>
{
    public void Configure(EntityTypeBuilder<VinculoPessoaImovel> b)
    {
        b.ToTable("vinculos_pessoa_imovel");
        b.HasKey(x => new { x.PessoaId, x.ImovelId, x.PerfilVinculoImovelId });


        b.HasOne(x => x.Pessoa)
        .WithMany(x => x.Vinculos)
        .HasForeignKey(x => x.PessoaId)
        .OnDelete(DeleteBehavior.Cascade);


        b.HasOne(x => x.Imovel)
        .WithMany(x => x.Vinculos)
        .HasForeignKey(x => x.ImovelId)
        .OnDelete(DeleteBehavior.Cascade);


        b.HasOne(x => x.Perfil)
        .WithMany(x => x.Vinculos)
        .HasForeignKey(x => x.PerfilVinculoImovelId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}