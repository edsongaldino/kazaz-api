using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IFotoService
{
    Task<IReadOnlyList<FotoListDto>> ListarPorImovelAsync(Guid imovelId, CancellationToken ct);
    Task<FotoListDto?> ObterAsync(Guid id, CancellationToken ct);
    Task<Guid> CriarAsync(FotoCreateDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, FotoUpdateDto dto, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
    Task ReordenarAsync(Guid imovelId, IEnumerable<(Guid fotoId, int ordem)> novaOrdem, CancellationToken ct);
}
