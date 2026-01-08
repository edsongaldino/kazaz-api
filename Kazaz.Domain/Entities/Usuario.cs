using System.ComponentModel.DataAnnotations;

namespace Kazaz.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(255)]
        public required string Nome { get; set; }

        [Required, MaxLength(150), EmailAddress]
        public required string Email { get; set; }

        [Required, MaxLength(255)]
        public required string Senha { get; set; }

        [Required]
        public required bool Ativo { get; set; }

        public Guid PerfilId { get; set; }     // FK única
        public Perfil Perfil { get; set; } = null!; // navegação única
    }
}