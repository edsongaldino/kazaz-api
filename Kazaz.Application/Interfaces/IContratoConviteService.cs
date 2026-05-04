using Kazaz.Application.DTOs;
using Kazaz.Domain.Entities;

namespace Kazaz.Application.Contracts;

public interface IContratoConviteService
{
    Task<GerarLinksContratoResponse> GerarLinksAsync(
        Guid imovelId,
        GerarLinksContratoRequest request,
        CancellationToken cancellationToken
    );

    Task AnalisarConviteAsync(Guid conviteId, ResultadoAnaliseConvite resultado, Guid usuarioId, string? comentario,  CancellationToken ct);


    Task<PagedResult<ConviteCadastroListItemResponse>> ListarAsync(ListarConvitesCadastroQuery query, CancellationToken ct);

}
