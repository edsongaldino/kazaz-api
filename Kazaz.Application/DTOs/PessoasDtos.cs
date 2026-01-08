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
public record DadosPessoaFisicaDto(
    string Cpf,
    string Rg,
    string Nome,
    string OrgaoExpedidor,
    DateOnly? DataNascimento,
    EstadoCivil? EstadoCivil,
    string? Nacionalidade
);

public sealed record PessoaFisicaUpdateDto(
    string? Nome,
    string? Cpf,
    DateOnly? DataNascimento,
    string? Rg,
    string OrgaoExpedidor,
    string? Nacionalidade,
    EstadoCivil? EstadoCivil
);

// PJ
public record DadosPessoaJuridicaDto(
    string NomeFantasia,
    string RazaoSocial,
    string Cnpj,
    string? InscricaoEstadual
);

public sealed record PessoaJuridicaUpdateDto(
    string? RazaoSocial,
    string? NomeFantasia,
    string? Cnpj,
    string? InscricaoEstadual
);


public record PessoaCreateDto(
    string Tipo,
    string Documento,
    EnderecoCreateDto Endereco,
    Guid? OrigemId,
    DadosPessoaFisicaDto? DadosPessoaFisica,
    DadosPessoaJuridicaDto? DadosPessoaJuridica,
    List<ContatoDto>? Contatos,
	DadosComplementaresDto? DadosComplementares,
    ConjugeDto? Conjuge
);

public sealed record PessoaDetailsDto(
    Guid Id,
    string TipoPessoa, // "PF" | "PJ" (ou "FISICA" | "JURIDICA")
    string? Nome,
    string? Documento,
    Guid? OrigemId,

    EnderecoResponseDto? Endereco,

    DadosPessoaFisicaDto? DadosPessoaFisica,
    DadosPessoaJuridicaDto? DadosPessoaJuridica,

    List<ContatoDto>? Contatos,

    DadosComplementaresDto? DadosComplementares,
    ConjugeDto? Conjuge
);

public sealed record PessoaUpdateDto(
    Guid Id,
    string Tipo,
    string? Documento,
    Guid? OrigemId,
    Guid? EnderecoId,
    EnderecoUpdateDto? Endereco,
    PessoaFisicaUpdateDto? DadosPessoaFisica,
    PessoaJuridicaUpdateDto? DadosPessoaJuridica,
    List<ContatoUpdateDto>? Contatos,
    DadosComplementaresUpdateDto? DadosComplementares,
    ConjugeUpdateDto? Conjuge
);