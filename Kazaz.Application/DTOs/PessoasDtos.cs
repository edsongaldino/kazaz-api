namespace Kazaz.Application.DTOs;

// Listagem unificada
public record PessoaListDto(
    Guid Id,
    string Tipo,            // "FISICA" ou "JURIDICA"
    string Nome,
    string? Documento,      // CPF ou CNPJ
    DateOnly? DataNascimento,   // apenas PF
    string? RazaoSocial,    // apenas PJ
    Guid? EnderecoId,
    Guid? OrigemId
);

// PF
public record PessoaFisicaCreateDto(
    string Nome,
    string Cpf,
    DateOnly? DataNascimento,
    Guid? EnderecoId,
    Guid? OrigemId
);

public record PessoaFisicaUpdateDto(
    string Nome,
    string Cpf,
    DateOnly? DataNascimento,
    Guid? EnderecoId,
    Guid? OrigemId
);

// PJ
public record PessoaJuridicaCreateDto(
    string Nome,
    string RazaoSocial,
    string Cnpj,
    Guid? EnderecoId,
    Guid? OrigemId
);

public record PessoaJuridicaUpdateDto(
    string Nome,
    string RazaoSocial,
    string Cnpj,
    Guid? EnderecoId,
    Guid? OrigemId
);


public record PessoaCreateDto(
    string Tipo,
    string Documento,
    string? Nome,
    string? RazaoSocial,
    EnderecoCreateDto Endereco,
    Guid? OrigemId,
    PessoaFisicaCreateDto? DadosPessoaFisica,
    PessoaJuridicaCreateDto? DadosPessoaJuridica,
    List<ContatoCreateDto>? Contatos,
	DadosComplementaresCreateDto? DadosComplementares,
    ConjugeCreateDto? Conjuge
);

public record PessoaDetailsDto(
    Guid Id,
    string TipoPessoa,     // "FISICA" ou "JURIDICA"
    string? Nome,
    string? Documento,     // CPF ou CNPJ conforme o tipo
    DateOnly? DataNascimento,
    string? RazaoSocial,
    EnderecoResponseDto? Endereco,
    Guid? OrigemId
);

public sealed record PessoaUpdateDto(
    string Tipo,               // "PF" | "PJ"  (aceito também FISICA/JURIDICA)
    string? Nome,              // PF: Nome | PJ: Nome Fantasia
    string? RazaoSocial,       // PJ
    string? Documento,         // PF: CPF | PJ: CNPJ
    DateOnly? DataNascimento,  // PF
    EnderecoUpdateDto? Endereco,
    Guid? OrigemId
);