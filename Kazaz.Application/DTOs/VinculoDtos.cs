namespace Kazaz.Application.DTOs;

public record VincularPessoaImovelDto(
    Guid PessoaId,
    Guid ImovelId,
    Guid PerfilVinculoImovelId
);