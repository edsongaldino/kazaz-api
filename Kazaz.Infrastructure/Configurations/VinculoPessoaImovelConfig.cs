using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public sealed class VinculoPessoaImovelConfig : IEntityTypeConfiguration<VinculoPessoaImovel>
{
	public void Configure(EntityTypeBuilder<VinculoPessoaImovel> b)
	{
		b.ToTable("vinculos_pessoa_imovel");

		b.HasKey(x => x.Id);

		b.Property(x => x.Id).HasColumnName("id");

		b.Property(x => x.PessoaId).HasColumnName("pessoa_id").IsRequired();
		b.Property(x => x.ImovelId).HasColumnName("imovel_id").IsRequired();
		b.Property(x => x.PerfilVinculoImovelId).HasColumnName("perfil_vinculo_imovel_id").IsRequired();

		// ✅ Evita duplicar o mesmo vínculo (pessoa + imóvel + perfil)
		b.HasIndex(x => new { x.PessoaId, x.ImovelId, x.PerfilVinculoImovelId })
			.IsUnique()
			.HasDatabaseName("ux_vinculos_pessoa_imovel_unico");

		b.HasOne(x => x.Pessoa)
			.WithMany(p => p.Vinculos) // confirme o nome da coleção em Pessoa
			.HasForeignKey(x => x.PessoaId)
			.OnDelete(DeleteBehavior.Restrict); // ✅ recomendado

		b.HasOne(x => x.Imovel)
			.WithMany(i => i.Vinculos)
			.HasForeignKey(x => x.ImovelId)
			.OnDelete(DeleteBehavior.Cascade); // ✅ deletou imóvel, apaga vínculos

		b.HasOne(x => x.Perfil)
			.WithMany(pf => pf.Vinculos)
			.HasForeignKey(x => x.PerfilVinculoImovelId)
			.OnDelete(DeleteBehavior.Restrict);

		// índices úteis para busca
		b.HasIndex(x => x.PessoaId).HasDatabaseName("ix_vinculos_pessoa_id");
		b.HasIndex(x => x.ImovelId).HasDatabaseName("ix_vinculos_imovel_id");
		b.HasIndex(x => x.PerfilVinculoImovelId).HasDatabaseName("ix_vinculos_perfil_id");
	}
}
