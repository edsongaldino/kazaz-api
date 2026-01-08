using Kazaz.Domain.Entities;

namespace Kazaz.Application.Contracts;

public interface IContratoConviteService
{
    Task<GerarLinksContratoResponse> GerarLinksAsync(
        Guid imovelId,
        GerarLinksContratoRequest request,
        CancellationToken cancellationToken
    );
}
