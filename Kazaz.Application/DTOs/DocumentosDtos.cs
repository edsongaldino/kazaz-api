namespace Kazaz.Application.DTOs;

// Documento pertence a Pessoa OU a Imovel (exatamente um)
public enum DocumentoAlvo
{
    Pessoa = 1,
    Imovel = 2
}

public record DocumentoListDto(
    Guid DocumentoId,
    string Nome,
    string Caminho,
    string? ContentType,
    long? TamanhoBytes,
    DateTime DataUpload,
    AlvoDocumento Alvo,
    Guid AlvoId,
    Guid TipoDocumentoId,
    string TipoDocumentoNome,
    bool TipoObrigatorio,
    int TipoOrdem,
    string? Observacao
);

public record DocumentoCreateDto(
    string Nome,
    string Caminho,
    string? ContentType,
    long? TamanhoBytes,
    AlvoDocumento Alvo,
    Guid AlvoId,                // PessoaId OU ImovelId
    Guid TipoDocumentoId,       // ex.: RG, CPF, Comprovante de Endereço, Matrícula do imóvel...
    string? Observacao = null
);

public record DocumentoUpdateDto(
    string Nome,
    string? Caminho,
    string? ContentType,
    long? TamanhoBytes
);

// Tipos
public record TipoDocumentoCreateDto(string Nome, AlvoDocumento Alvo, bool Obrigatorio, int Ordem, string? Descricao);
public record TipoDocumentoUpdateDto(string Nome, bool Obrigatorio, int Ordem, bool Ativo, string? Descricao);

// Anexos Pessoa
public record AnexarDocumentoPessoaDto(Guid PessoaId, Guid TipoDocumentoId, string Nome, string Caminho, string? ContentType, long? TamanhoBytes, string? Observacao);

// Anexos Imóvel
public record AnexarDocumentoImovelDto(Guid ImovelId, Guid TipoDocumentoId, string Nome, string Caminho, string? ContentType, long? TamanhoBytes, string? Observacao);
