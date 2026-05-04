using Kazaz.Domain.Entities;

public record GerarLinksContratoRequest(
    TipoContrato Tipo,
    PapelContrato Papel,
    int ExpiraEmDias
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
    TipoContrato TipoContrato,
    StatusContrato StatusContrato,
    Guid ImovelId,
    string? NomeImovel,
    PapelContrato Papel,
    StatusConviteCadastro Status,
    string Token,
    string Link,
    DateTime CriadoEm,
    DateTime? ExpiraEm,
    DateTime? UsadoEm,
    DateTime? PreenchidoEm,
    Guid? PessoaId,
    string? NomePessoa,
    string? Documento,
    string? UltimoComentarioAnalise
);


public class ListarConvitesCadastroQuery
{
    public Guid? ContratoId { get; set; }
    public Guid? ImovelId { get; set; }
    public StatusConviteCadastro? Status { get; set; }
    public PapelContrato? Papel { get; set; }

    public string? Nome { get; set; }
    public string? Documento { get; set; }
    public string? Imovel { get; set; }
    public DateTime? PreenchidoDe { get; set; }
    public DateTime? PreenchidoAte { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
