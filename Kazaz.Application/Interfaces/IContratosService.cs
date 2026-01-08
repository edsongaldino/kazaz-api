using Kazaz.Application.DTOs;
namespace Kazaz.Application.Services.Interfaces;

public interface IContratosService
{
    Task<ContratoResponse> CriarRascunhoAsync(CriarContratoRequest req, CancellationToken ct);
    Task<ContratoResponse> AtivarAsync(Guid contratoId, CancellationToken ct);
    Task<ContratoResponse> CancelarAsync(Guid contratoId, CancellationToken ct);
    Task<ContratoResponse> EncerrarAsync(Guid contratoId, CancellationToken ct);

    Task<ContratoResponse> ObterPorIdAsync(Guid id, CancellationToken ct);

    Task<List<ContratoResponse>> ListarAsync(
        Guid? imovelId,
        TipoContrato? tipo,
        StatusContrato? status,
        CancellationToken ct);
}
