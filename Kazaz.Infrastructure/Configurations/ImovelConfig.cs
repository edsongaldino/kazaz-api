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

		b.Property(x => x.TipoImovelId).HasColumnName("tipo_imovel_id");
		b.HasOne(x => x.TipoImovel)
		 .WithMany(t => t.Imoveis)
		 .HasForeignKey(x => x.TipoImovelId);

		b.Property(x => x.Finalidade)
	    .HasColumnName("finalidade")
	    .HasConversion<int>()
	    .IsRequired();

		b.Property(x => x.Status)
			.HasColumnName("status")
			.HasConversion<int>()
			.IsRequired();

		// 1:1 obrigatório com Endereco
		b.HasOne(x => x.Endereco)
        .WithMany()
        .HasForeignKey(x => x.EnderecoId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired();

		// 🔹 Características (1:N)
		b.HasMany(x => x.Caracteristicas)
			.WithOne(x => x.Imovel)
			.HasForeignKey(x => x.ImovelId)
			.OnDelete(DeleteBehavior.Cascade);

		// 🔹 Fotos (1:N)
		b.HasMany(x => x.Fotos)
			.WithOne(x => x.Imovel)
			.HasForeignKey(x => x.ImovelId)
			.OnDelete(DeleteBehavior.Cascade);

		// 🔹 Documentos (1:N) ✅ FALTAVA
		b.HasMany(x => x.Documentos)
			.WithOne(x => x.Imovel)
			.HasForeignKey(x => x.ImovelId)
			.OnDelete(DeleteBehavior.Cascade);

		b.HasMany(x => x.Vinculos)
			.WithOne(x => x.Imovel)
			.HasForeignKey(x => x.ImovelId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}