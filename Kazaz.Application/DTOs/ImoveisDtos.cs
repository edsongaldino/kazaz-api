namespace Kazaz.Application.DTOs;

public record ImovelListDto(
    Guid Id,
    string Codigo,
    Guid EnderecoId
);

public record ImovelUpsertDto(
	string Codigo,
	string? Titulo,
	FinalidadeImovel Finalidade,
	StatusImovel Status,
	Guid TipoImovelId,
	EnderecoCreateDto Endereco,
	string? Observacoes,
	List<ImovelCaracteristicaUpsertDto> Caracteristicas,
	List<VinculoPessoaImovelUpsertDto> Vinculos
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
	List<ImovelCaracteristicaDto> Caracteristicas,
	List<VinculoPessoaImovelDto> Vinculos,
	List<ImovelFotoDto> Fotos,
	List<ImovelDocumentoDto> Documentos
);