using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Infrastructure.Configurations;

public class ImovelCaracteristicaConfig : IEntityTypeConfiguration<ImovelCaracteristica>
{
	public void Configure(EntityTypeBuilder<ImovelCaracteristica> b)
	{
		b.ToTable("imoveis_caracteristicas");
		b.HasKey(x => x.Id);

		b.Property(x => x.ImovelId).HasColumnName("imovel_id");
		b.Property(x => x.CaracteristicaId).HasColumnName("caracteristica_id");

		b.Property(x => x.ValorBool).HasColumnName("valor_bool");
		b.Property(x => x.ValorInt).HasColumnName("valor_int");
		b.Property(x => x.ValorDecimal).HasColumnName("valor_decimal").HasPrecision(14, 2);
		b.Property(x => x.ValorTexto).HasColumnName("valor_texto").HasMaxLength(255);
		b.Property(x => x.ValorData).HasColumnName("valor_data");
		b.Property(x => x.Observacao).HasColumnName("observacao").HasMaxLength(255);

		b.HasOne(x => x.Imovel)
			.WithMany(x => x.Caracteristicas)
			.HasForeignKey(x => x.ImovelId)
			.OnDelete(DeleteBehavior.Cascade);

		b.HasOne(x => x.Caracteristica)
			.WithMany(x => x.ImoveisCaracteristicas)
			.HasForeignKey(x => x.CaracteristicaId)
			.OnDelete(DeleteBehavior.Restrict);

		b.HasIndex(x => new { x.ImovelId, x.CaracteristicaId })
			.IsUnique()
			.HasDatabaseName("ux_ic_imovel_caracteristica");
	}
}

