using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface ICadastroPublicoService
{
    Task<ConvitePublicInfoResponse> ObterConviteAsync(string token, CancellationToken ct);

    // Reaproveitando seu DTO:
    // - aqui ele serve como "iniciar": cria/atualiza Pessoa + ContratoParte
    // - request.Documentos pode vir vazio (recomendado)
    Task<FinalizarCadastroPublicoResponse> IniciarAsync(
        string token,
        FinalizarCadastroPublicoRequest request,
        CancellationToken ct
    );

    // Marca convite como usado (e depois a gente pluga validação obrigatórios)
    Task<FinalizarCadastroPublicoResponse> ConcluirAsync(string token, CancellationToken ct);
}
