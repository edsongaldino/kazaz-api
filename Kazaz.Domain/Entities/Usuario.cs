using System.ComponentModel.DataAnnotations;

namespace Kazaz.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150), EmailAddress]
        public required string Email { get; set; }

        [Required, MaxLength(255)]
        public required string Senha { get; set; }

        public Guid? PerfilId { get; set; }
        public Perfil? Perfil { get; set; }
    }
}