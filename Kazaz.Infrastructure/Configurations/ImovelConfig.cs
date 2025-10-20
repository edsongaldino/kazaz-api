using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Kazaz.Infrastructure.Configurations;


public sealed class ImovelConfig : IEntityTypeConfiguration<Imovel>
{
    public void Configure(EntityTypeBuilder<Imovel> b)
    {
        b.ToTable("imoveis");
        b.HasKey(x => x.Id);
        b.Property(x => x.Codigo).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.Codigo).IsUnique();

        // 1:1 obrigatório com Endereco
        b.HasOne(x => x.Endereco)
        .WithMany()
        .HasForeignKey(x => x.EnderecoId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired();

        b.HasMany(x => x.Fotos)
        .WithOne(x => x.Imovel)
        .HasForeignKey(x => x.ImovelId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}