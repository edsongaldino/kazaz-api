using Kazaz.Domain.Entities;
using System;

namespace Kazaz.Application.DTOs;

public record RegraDocumentoCadastroResponse(
    Guid Id,
    TipoPessoaRule TipoPessoa,
    TipoContratoRule TipoContrato,
    PapelContratoRule PapelContrato,
    Guid TipoDocumentoId,
    string TipoDocumentoNome,
    bool Obrigatorio,
    int Ordem,
    int Multiplicidade,
    string? Rotulo,
    bool Ativo
);

public record CriarRegraDocumentoCadastroRequest(
    TipoPessoaRule TipoPessoa,
    TipoContratoRule TipoContrato,
    PapelContratoRule PapelContrato,
    Guid TipoDocumentoId,
    bool Obrigatorio,
    int Ordem,
    int Multiplicidade,
    string? Rotulo,
    bool Ativo
);

public record AtualizarRegraDocumentoCadastroRequest(
    TipoPessoaRule TipoPessoa,
    TipoContratoRule TipoContrato,
    PapelContratoRule PapelContrato,
    Guid TipoDocumentoId,
    bool Obrigatorio,
    int Ordem,
    int Multiplicidade,
    string? Rotulo,
    bool Ativo
);
