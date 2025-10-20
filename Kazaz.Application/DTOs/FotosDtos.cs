namespace Kazaz.Application.DTOs;

public record FotoListDto(
    Guid Id,
    Guid ImovelId,
    string Caminho,
    int Ordem
);

public record FotoCreateDto(
    Guid ImovelId,
    string Caminho,
    int Ordem
);

public record FotoUpdateDto(
    string Caminho,
    int Ordem
);