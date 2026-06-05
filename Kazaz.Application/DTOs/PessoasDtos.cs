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
    Guid? OrigemId,

    int QuantidadeContratos,
    bool EhLocador,
    bool EhLocatario,
    bool EhFiador,
    bool EhVendedor,
    bool EhComprador,
    bool EhProprietario
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
    string InscricaoEstadual,
    DateOnly? DataAbertura
);

public sealed record PessoaJuridicaUpdateDto(
    string? RazaoSocial,
    string? NomeFantasia,
    string? Cnpj,
    string? InscricaoEstadual,
    DateOnly? DataAbertura
);


public record PessoaCreateDto(
    Guid? PessoaId,
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
    List<ContatoDto>? Contatos,
    DadosComplementaresDto? DadosComplementares,
    ConjugeUpdateDto? Conjuge
);

public record CadastroPublicoDetalhesResponse(
    PessoaDetailsDto? Pessoa,
    IReadOnlyList<DocumentoVisualizacaoDto> Documentos
);

public record DocumentoVisualizacaoDto(
    Guid Id,
    Guid TipoDocumentoId,
    string TipoDocumentoNome,
    string Nome,
    string Caminho,
    string? ContentType
);

public class PessoaFiltroDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Nome { get; set; }
    public string? Documento { get; set; }

    public string? Tipo { get; set; } // 1 = Fisica, 2 = Juridica
    public int? Papel { get; set; } // enum PapelContrato
}