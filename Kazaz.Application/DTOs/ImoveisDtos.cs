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
	List<ImovelDocumentoDto> Documentos
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