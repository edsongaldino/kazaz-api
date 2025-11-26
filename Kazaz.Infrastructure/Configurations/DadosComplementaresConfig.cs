using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public class DadosComplementaresConfig : IEntityTypeConfiguration<DadosComplementares>
{
	public void Configure(EntityTypeBuilder<DadosComplementares> builder)
	{
		builder.ToTable("dados_complementares");

		builder.HasKey(d => d.Id);

		builder.Property(d => d.Profissao)
			.HasMaxLength(150);

		builder.Property(d => d.Escolaridade)
			.HasMaxLength(100);

		builder.Property(d => d.RendaMensal)
			.HasColumnType("numeric(12,2)");

		builder.Property(d => d.Observacoes)
			.HasMaxLength(1000);

		builder.Property(d => d.DataCriacao)
			.IsRequired();

		builder.HasOne(d => d.Pessoa)
			.WithOne(p => p.DadosComplementares)
			.HasForeignKey<DadosComplementares>(d => d.PessoaId);
	}
}

