using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kazaz.Infrastructure.Configurations;

public class ColaboradorConfiguration : IEntityTypeConfiguration<Colaborador>
{
    public void Configure(EntityTypeBuilder<Colaborador> b)
    {
        b.ToTable("colaboradores");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Cpf)
            .HasColumnName("cpf")
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.Cargo)
            .HasColumnName("cargo")
            .HasConversion<string>()
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Telefone)
            .HasColumnName("telefone")
            .HasMaxLength(50);

        b.Property(x => x.Ativo)
            .HasColumnName("ativo")
            .HasDefaultValue(true)
            .IsRequired();

        b.Property(x => x.DataAdmissao)
            .HasColumnName("data_admissao");

        b.Property(x => x.UsuarioId)
            .HasColumnName("usuario_id");

        b.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        b.Property(x => x.ImobiliariaId)
            .HasColumnName("imobiliaria_id");

        b.HasOne(x => x.Imobiliaria)
            .WithMany()
            .HasForeignKey(x => x.ImobiliariaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Cpf).IsUnique().HasDatabaseName("ix_colaboradores_cpf");
        b.HasIndex(x => x.UsuarioId).HasDatabaseName("ix_colaboradores_usuario_id");
    }
}
