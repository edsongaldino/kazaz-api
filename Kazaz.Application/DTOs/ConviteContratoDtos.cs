using Kazaz.Domain.Entities;

public record GerarLinksContratoRequest(
    TipoContrato Tipo,
    int ExpiraEmDias = 7,
    bool IncluirFiador = false
);

public record ConviteLinkResponse(
    PapelContrato Papel,
    string Token,
    string Url
);

public record GerarLinksContratoResponse(
    Guid ContratoId,
    string Numero,
    IReadOnlyList<ConviteLinkResponse> Links
);


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

public record CadastroPublicoStatusResponse(
    Guid ContratoId,
    Guid? PessoaId,
    PapelContrato Papel,
    bool Concluido,
    bool Iniciado
);


public record ConviteCadastroListItemResponse(
    Guid Id,
    Guid ContratoId,
    string NumeroContrato,
    TipoContrato Tipo,
    PapelContrato Papel,
    StatusConviteCadastro Status,
    string Token,
    string Url,
    DateTime CriadoEm,
    DateTime? ExpiraEm,
    DateTime? UsadoEm,
    Guid? PessoaId
);


public record ListarConvitesCadastroQuery(
    Guid? ContratoId = null,
    StatusConviteCadastro? Status = null,
    PapelContrato? Papel = null,
    int Page = 1,
    int PageSize = 50
);
