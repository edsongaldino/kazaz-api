using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContratoConfig : IEntityTypeConfiguration<Contrato>
{
    public void Configure(EntityTypeBuilder<Contrato> b)
    {
        b.ToTable("contratos");
        b.HasKey(x => x.Id);

        b.Property(x => x.Tipo).HasColumnName("tipo").HasConversion<int>();
        b.Property(x => x.Status).HasColumnName("status").HasConversion<int>();

        b.Property(x => x.ImovelId).HasColumnName("imovel_id");

        b.Property(x => x.InicioVigencia).HasColumnName("inicio_vigencia");
        b.Property(x => x.FimVigencia).HasColumnName("fim_vigencia");

        b.Property(x => x.Numero).HasColumnName("numero").HasMaxLength(30).IsRequired();
        b.HasIndex(x => x.Numero).IsUnique();

        b.Property(x => x.CriadoEm).HasColumnName("criado_em");

        b.HasOne(x => x.Imovel)
            .WithMany()
            .HasForeignKey(x => x.ImovelId);

        b.HasMany(x => x.Partes)
            .WithOne(x => x.Contrato)
            .HasForeignKey(x => x.ContratoId);
    }
}
