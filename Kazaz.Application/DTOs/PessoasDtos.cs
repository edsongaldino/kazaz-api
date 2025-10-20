namespace Kazaz.Application.DTOs;

// Listagem unificada
public record PessoaListDto(
    Guid Id,
    string Tipo,            // "FISICA" ou "JURIDICA"
    string Nome,
    string? Documento,      // CPF ou CNPJ
    DateOnly? Nascimento,   // apenas PF
    string? RazaoSocial,    // apenas PJ
    Guid? EnderecoId
);

// PF
public record PessoaFisicaCreateDto(
    string Nome,
    string Cpf,
    DateOnly? Nascimento,
    Guid? EnderecoId
);

public record PessoaFisicaUpdateDto(
    string Nome,
    string Cpf,
    DateOnly? Nascimento,
    Guid? EnderecoId
);

// PJ
public record PessoaJuridicaCreateDto(
    string Nome,
    string RazaoSocial,
    string Cnpj,
    Guid? EnderecoId
);

public record PessoaJuridicaUpdateDto(
    string Nome,
    string RazaoSocial,
    string Cnpj,
    Guid? EnderecoId
);


public record PessoaCreateDto(
    string Tipo, 
    string Documento, 
    string? Nome,
    string? RazaoSocial,
    DateOnly? Nascimento,
    EnderecoCreateDto Endereco
);