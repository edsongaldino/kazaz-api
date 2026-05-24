// ---- ImovelProprietario DTOs ----

public record ImovelProprietarioDto(
    Guid Id,
    Guid PessoaId,
    string PessoaNome,
    string? PessoaDocumento,
    decimal? Percentual,
    bool Ativo
);

public record AdicionarProprietarioRequest(
    Guid PessoaId,
    decimal? Percentual
);
