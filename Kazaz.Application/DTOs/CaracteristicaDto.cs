public sealed record CaracteristicaListDto(
    Guid Id,
    string Nome,
    string TipoValor,
    string? Unidade,
    string? Grupo,
    int Ordem,
    bool Ativo
);

public sealed record GrupoCaracteristicaDto(string Nome);
