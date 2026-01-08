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