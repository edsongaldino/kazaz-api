using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IImovelService
{
    Task<(IReadOnlyList<ImovelListDto> Items, int Total)> ListarAsync(int page, int pageSize, string? termo, CancellationToken ct);
    Task<ImovelListDto?> ObterAsync(Guid id, CancellationToken ct);
    Task<Guid> CriarAsync(ImovelUpsertDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, ImovelUpsertDto dto, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
}
