using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Infrastructure.Configurations;

public class CaracteristicaConfig : IEntityTypeConfiguration<Caracteristica>
{
	public void Configure(EntityTypeBuilder<Caracteristica> b)
	{
		b.ToTable("caracteristicas");
		b.HasKey(x => x.Id);

		b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
		b.Property(x => x.TipoValor).HasColumnName("tipo_valor").HasMaxLength(20).IsRequired();
		b.Property(x => x.Unidade).HasColumnName("unidade").HasMaxLength(20);
		b.Property(x => x.Grupo).HasColumnName("grupo").HasMaxLength(50);
		b.Property(x => x.Ordem).HasColumnName("ordem").HasDefaultValue(0);
		b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
		b.Property(x => x.CreatedAt).HasColumnName("created_at");

		b.HasIndex(x => x.Nome).IsUnique().HasDatabaseName("ux_caracteristicas_nome");
	}
}

