using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UsuarioConfi : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("usuarios");

        b.HasKey(x => x.Id);

        b.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(200)
            .IsRequired();

        b.HasIndex(x => x.Email)
            .IsUnique();

        b.Property(x => x.Senha)
            .HasColumnName("senha")
            .HasMaxLength(500)
            .IsRequired();

        b.Property(x => x.Ativo)
            .HasColumnName("ativo")
            .HasDefaultValue(true)
            .IsRequired();

        b.HasOne(u => u.Perfil)
            .WithMany(p => p.Usuarios)
            .HasForeignKey(u => u.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Property(u => u.PerfilId).HasColumnName("perfil_id");
    }
}
