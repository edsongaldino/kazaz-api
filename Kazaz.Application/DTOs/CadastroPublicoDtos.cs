using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public record ConvitePublicInfoResponse(
    bool Valido,
    string? Motivo,
    Guid? ContratoId,
    string? NumeroContrato,
    TipoContrato? Tipo,
    PapelContrato? Papel,
    DateTime? ExpiraEm,
    Guid? ImovelId
);

public record PessoaDocumentoInput(
    Guid TipoDocumentoId,
    Guid DocumentoId,
    string? Observacao
);

public record FinalizarCadastroPublicoRequest(
    string Nome,
    Guid? EnderecoId,
    List<PessoaDocumentoInput> Documentos
);

public record FinalizarCadastroPublicoResponse(
    Guid ContratoId,
    Guid PessoaId,
    PapelContrato Papel
);
