using Kazaz.Application.DTOs;
namespace Kazaz.Application.Services.Interfaces;

public interface IContratosService
{
    Task<ContratoResponse> CriarRascunhoAsync(CriarContratoRequest req, CancellationToken ct);
    Task<ContratoResponse> AtivarAsync(Guid contratoId, CancellationToken ct);
    Task<ContratoResponse> CancelarAsync(Guid contratoId, CancellationToken ct);
    Task<ContratoResponse> EncerrarAsync(Guid contratoId, CancellationToken ct);

    Task<ContratoResponse> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<ContratoResponse> AtualizarAsync(Guid id, AtualizarContratoRequest req, CancellationToken ct);

    Task<PagedResult<ContratoResponse>> ListarAsync(
     ListarContratosQuery query,
     CancellationToken ct);

    Task<ContratoChecklistEntradaResponse> ObterChecklistEntradaAsync(Guid contratoId, CancellationToken ct);
    Task<ContratoChecklistEntradaResponse> SalvarChecklistEntradaAsync(Guid contratoId, SalvarChecklistEntradaRequest req, CancellationToken ct);

    Task<ContratoChecklistSaidaResponse> ObterChecklistSaidaAsync(Guid contratoId, CancellationToken ct);
    Task<ContratoChecklistSaidaResponse> SalvarChecklistSaidaAsync(Guid contratoId, SalvarChecklistSaidaRequest req, CancellationToken ct);
}
