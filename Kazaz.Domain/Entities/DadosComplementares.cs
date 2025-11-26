namespace Kazaz.Domain.Entities; 
public sealed class DadosComplementares
{
	public Guid Id { get; set; }

	public Guid PessoaId { get; set; }
	public Pessoa Pessoa { get; set; } = null!;

	public string? Profissao { get; set; }
	public string? Escolaridade { get; set; }
	public decimal? RendaMensal { get; set; }
	public string? Observacoes { get; set; }

	public DateTime DataCriacao { get; set; }
	public DateTime? DataAtualizacao { get; set; }
}