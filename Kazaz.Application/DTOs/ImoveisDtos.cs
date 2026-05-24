namespace Kazaz.Application.DTOs;

public record ImovelListDto(
    Guid Id,
    string Codigo,
	string Titulo,
    FinalidadeImovel Finalidade,
    StatusImovel Status,
    string TipoImovelNome,
    EnderecoListDto? Endereco
);

public record ImovelUpsertDto(
	string Codigo,
	string? Titulo,
	FinalidadeImovel Finalidade,
	StatusImovel Status,
	Guid TipoImovelId,
	EnderecoCreateDto Endereco,
	string? Observacoes,
	List<ImovelCaracteristicaUpsertDto> Caracteristicas
);

public sealed record ImovelDetailsDto(
	Guid Id,
	string Codigo,
	string? Titulo,
	FinalidadeImovel Finalidade,
	StatusImovel Status,
	Guid TipoImovelId,
	string TipoImovelNome,
	Guid EnderecoId,
	string? Observacoes,
	Endereco? Endereco,
	List<ImovelCaracteristicaDto> Caracteristicas,
	List<ImovelContratoResumoDto> Contratos,
	List<ImovelFotoDto> Fotos,
	List<ImovelDocumentoDto> Documentos,
	List<ImovelProprietarioDto> Proprietarios
);

public sealed record ImovelContratoResumoDto(
    Guid Id,
    string Numero,
    TipoContrato Tipo,
    StatusContrato Status,
    DateOnly InicioVigencia,
    DateOnly? FimVigencia,
    List<ImovelContratoParteDto> Partes
);

public sealed record ImovelContratoParteDto(
    Guid PessoaId,
    string PessoaNome,
    PapelContrato Papel,
    decimal? Percentual
);

public class ListarImoveisQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Termo { get; set; } // Codigo ou titulo
    public string? DocumentoProprietario { get; set; }

    public Guid? TipoImovelId { get; set; }
    public int? Finalidade { get; set; }
    public Guid? CidadeId { get; set; }
    public int? Status { get; set; }
}