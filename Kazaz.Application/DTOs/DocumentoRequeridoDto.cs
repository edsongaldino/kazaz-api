using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public class DocumentoRequeridoDto
{
    public Guid TipoDocumentoId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public bool Obrigatorio { get; init; }
    public int Ordem { get; init; }

    // quando Multiplicidade > 1, retornamos linhas duplicadas com indice 1..N
    public int? MultiplicidadeIndex { get; init; }
}

public class DocumentosRequeridosResponse
{
    public Guid ContratoId { get; init; }
    public Guid? PessoaId { get; init; }

    public int TipoContrato { get; init; }
    public int PapelContrato { get; init; }
    public string TipoPessoa { get; init; } = "PF";

    public List<DocumentoRequeridoDto> Itens { get; init; } = new();
}