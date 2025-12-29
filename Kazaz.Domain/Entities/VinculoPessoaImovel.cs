namespace Kazaz.Domain.Entities;

public sealed class VinculoPessoaImovel
{
	public Guid Id { get; set; } // ✅ PK

	public Guid PessoaId { get; set; }
	public Guid ImovelId { get; set; }
	public Guid PerfilVinculoImovelId { get; set; }

	public Pessoa Pessoa { get; set; } = default!;
	public Imovel Imovel { get; set; } = default!;
	public PerfilVinculoImovel Perfil { get; set; } = default!;
}
