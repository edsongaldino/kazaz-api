using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Kazaz.Infrastructure.Configurations;


public sealed class SocioConfig : IEntityTypeConfiguration<Socio>
{
    public void Configure(EntityTypeBuilder<Socio> b)
    {
        b.ToTable("socios");
        b.HasKey(x => x.Id);
        b.Property(x => x.Percentual).HasPrecision(9, 2);


        b.HasOne(x => x.PessoaFisica)
        .WithMany()
        .HasForeignKey(x => x.PessoaFisicaId)
        .OnDelete(DeleteBehavior.Restrict);


        b.HasIndex(x => new { x.EmpresaId, x.PessoaFisicaId }).IsUnique();
    }
}