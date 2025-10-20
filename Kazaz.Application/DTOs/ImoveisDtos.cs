namespace Kazaz.Application.DTOs;

public record ImovelListDto(
    Guid Id,
    string Codigo,
    Guid EnderecoId
);

public record ImovelCreateDto(
    string Codigo,
    Guid EnderecoId
);

public record ImovelUpdateDto(
    string Codigo,
    Guid EnderecoId
);