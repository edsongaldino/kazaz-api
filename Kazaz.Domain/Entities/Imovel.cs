using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public sealed class Imovel
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = default!;
	public string? Titulo { get; set; }
	public FinalidadeImovel Finalidade { get; set; }
	public StatusImovel Status { get; set; }
	public Guid EnderecoId { get; set; }
    public Endereco Endereco { get; set; } = default!;

	public string? Observacoes { get; set; }

	public Guid TipoImovelId { get; set; }
	public TipoImovel TipoImovel { get; set; } = null!;

	public ICollection<ImovelCaracteristica> Caracteristicas { get; set; } = new List<ImovelCaracteristica>();

	public List<VinculoPessoaImovel> Vinculos { get; set; } = new();
	public ICollection<Foto> Fotos { get; set; } = new List<Foto>();
	public ICollection<ImovelDocumento> Documentos { get; set; } = new List<ImovelDocumento>();
}
