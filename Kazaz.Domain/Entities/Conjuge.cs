namespace Kazaz.Domain.Entities;

public sealed class Conjuge
{
	public Guid Id { get; set; }

	public Guid PessoaId { get; set; }
	public Pessoa Pessoa { get; set; } = null!;

	public string Nome { get; set; } = null!;
	public string Cpf { get; set; } = null!;
	public string? Rg { get; set; }
	public string? OrgaoExpedidor { get; set; }
	public DateOnly? DataNascimento { get; set; }
	public string? Telefone { get; set; }
	public string? Email { get; set; }

	public DateTime DataCriacao { get; set; }
	public DateTime? DataAtualizacao { get; set; }
}