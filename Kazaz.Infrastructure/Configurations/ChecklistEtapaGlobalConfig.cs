using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public class ChecklistEtapaGlobalConfig : IEntityTypeConfiguration<ChecklistEtapaGlobal>
{
    public void Configure(EntityTypeBuilder<ChecklistEtapaGlobal> b)
    {
        b.ToTable("checklist_etapas_globais");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.TipoChecklist).HasColumnName("tipo_checklist").IsRequired().HasMaxLength(50);
        b.Property(x => x.Label).HasColumnName("label").IsRequired().HasMaxLength(250);
        b.Property(x => x.TipoField).HasColumnName("tipo_field").IsRequired().HasMaxLength(50);
        b.Property(x => x.Card).HasColumnName("card").IsRequired().HasMaxLength(150);

        b.Property(x => x.ImobiliariaId).HasColumnName("imobiliaria_id");
        b.HasOne(x => x.Imobiliaria)
            .WithMany()
            .HasForeignKey(x => x.ImobiliariaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
