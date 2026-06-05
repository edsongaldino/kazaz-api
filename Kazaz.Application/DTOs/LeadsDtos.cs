using System;

namespace Kazaz.Application.DTOs;

public record LeadResponseDto(
    Guid Id,
    string Nome,
    string? Email,
    string? Telefone,
    Guid? OrigemId,
    string? OrigemNome,
    Guid? ImovelId,
    string? ImovelCodigo,
    LeadStatus Status,
    string? Mensagem,
    Guid? PessoaId,
    DateTime DataCriacao,
    DateTime? DataAtualizacao
);

public record LeadCreateDto(
    string Nome,
    string? Email,
    string? Telefone,
    Guid? OrigemId,
    Guid? ImovelId,
    string? Mensagem,
    LeadStatus? Status
);

public record LeadUpdateDto(
    string Nome,
    string? Email,
    string? Telefone,
    Guid? OrigemId,
    Guid? ImovelId,
    string? Mensagem,
    LeadStatus Status
);

public record LeadSearchFilterDto(
    string? Nome,
    string? Email,
    string? Telefone,
    LeadStatus? Status,
    Guid? OrigemId,
    int Page = 1,
    int PageSize = 10
);

public record ConvertLeadRequest(
    string TipoPessoa, // "PF" or "PJ"
    string? Documento  // CPF or CNPJ
);

public record LeadsPagedResponse(
    IReadOnlyList<LeadResponseDto> Items,
    int Page,
    int PageSize,
    long Total,
    int TotalNovo,
    int TotalEmAtendimento,
    int TotalConvertido,
    int TotalDescartado
);
