using System;
using System.Collections.Generic;
using Kazaz.Domain.Entities;

namespace Kazaz.Application.DTOs;

// --- IMOBILIÁRIA DTOs ---
public record ImobiliariaResponseDto(
    Guid Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string Creci,
    DateTime? DataFundacao,
    string? LogoUrl,
    string? Email,
    string? Telefone,
    EnderecoResponseDto? Endereco
);

public record ImobiliariaUpdateDto(
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string Creci,
    DateTime? DataFundacao,
    string? LogoUrl,
    string? Email,
    string? Telefone,
    EnderecoRequest? Endereco
);

public record ImobiliariaCriarDto(
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string Creci,
    DateTime? DataFundacao,
    string? LogoUrl,
    string? Email,
    string? Telefone,
    EnderecoRequest? Endereco,
    string AdminNome,
    string AdminEmail,
    string AdminSenha
);

// --- COLABORADOR DTOs ---
public record ColaboradorDocumentoResponseDto(
    Guid Id,
    string Nome,
    Guid DocumentoId,
    string DocumentoNome,
    string Caminho,
    string? ContentType,
    long? TamanhoBytes,
    DateTime DataAnexo
);

public record ColaboradorDocumentoInputDto(
    Guid? Id,
    string Nome,
    Guid? DocumentoId,
    string? Caminho,
    string? DocumentoNome,
    string? ContentType,
    long? TamanhoBytes
);

public record ColaboradorResponseDto(
    Guid Id,
    string Nome,
    string Cpf,
    CargoColaborador Cargo,
    string Email,
    string? Telefone,
    bool Ativo,
    DateTime? DataAdmissao,
    Guid? UsuarioId,
    string? UsuarioEmail,
    List<ColaboradorDocumentoResponseDto> Documentos
);

public record ColaboradorCreateDto(
    string Nome,
    string Cpf,
    CargoColaborador Cargo,
    string Email,
    string? Telefone,
    bool Ativo,
    DateTime? DataAdmissao,
    bool CriarUsuario,
    string? Senha,
    Guid? PerfilId,
    List<ColaboradorDocumentoInputDto>? Documentos
);

public record ColaboradorUpdateDto(
    string Nome,
    string Cpf,
    CargoColaborador Cargo,
    string Email,
    string? Telefone,
    bool Ativo,
    DateTime? DataAdmissao,
    List<ColaboradorDocumentoInputDto>? Documentos
);

public record ColaboradorSearchFilterDto(
    string? Termo,
    bool? Ativo,
    int Page = 1,
    int PageSize = 10
);

// --- FINANCEIRO DTOs ---
public record FinanceiroLancamentoResponseDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    TipoLancamento Tipo,
    StatusLancamento Status,
    DateTime DataVencimento,
    DateTime? DataPagamento,
    string Categoria,
    Guid? ClienteId,
    string? ClienteNome,
    Guid? ContratoId,
    string? ContratoNumero
);

public record FinanceiroLancamentoCreateDto(
    string Descricao,
    decimal Valor,
    TipoLancamento Tipo,
    StatusLancamento Status,
    DateTime DataVencimento,
    DateTime? DataPagamento,
    string Categoria,
    Guid? ClienteId,
    Guid? ContratoId
);

public record FinanceiroLancamentoUpdateDto(
    string Descricao,
    decimal Valor,
    TipoLancamento Tipo,
    StatusLancamento Status,
    DateTime DataVencimento,
    DateTime? DataPagamento,
    string Categoria,
    Guid? ClienteId,
    Guid? ContratoId
);

public record FinanceiroLancamentoSearchFilterDto(
    TipoLancamento? Tipo,
    StatusLancamento? Status,
    string? Categoria,
    DateTime? DataInicio,
    DateTime? DataFim,
    int Page = 1,
    int PageSize = 10
);

public record FinanceiroResumoDto(
    decimal TotalReceberPendente,
    decimal TotalPagarPendente,
    decimal TotalRecebido,
    decimal TotalPago,
    decimal SaldoLiquido
);

// --- PRESTADOR SERVIÇO DTOs ---
public record PrestadorServicoResponseDto(
    Guid Id,
    string Nome,
    EspecialidadePrestador Especialidade,
    string CpfCnpj,
    string Telefone,
    string? Email,
    bool Ativo,
    string? Observacoes,
    EnderecoResponseDto? Endereco
);

public record PrestadorServicoCreateDto(
    string Nome,
    EspecialidadePrestador Especialidade,
    string CpfCnpj,
    string Telefone,
    string? Email,
    bool Ativo,
    string? Observacoes,
    EnderecoRequest? Endereco
);

public record PrestadorServicoUpdateDto(
    string Nome,
    EspecialidadePrestador Especialidade,
    string CpfCnpj,
    string Telefone,
    string? Email,
    bool Ativo,
    string? Observacoes,
    EnderecoRequest? Endereco
);

public record PrestadorServicoSearchFilterDto(
    string? Termo,
    EspecialidadePrestador? Especialidade,
    bool? Ativo,
    int Page = 1,
    int PageSize = 10
);
