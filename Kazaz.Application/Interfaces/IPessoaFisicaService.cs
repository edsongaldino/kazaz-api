
using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface IPessoaFisicaService
{
    Task<Guid> CriarAsync(PessoaFisicaCreateDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, PessoaFisicaUpdateDto dto, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<PessoaListDto> Items, int Total)> ListarAsync(int page, int pageSize, string? termo, CancellationToken ct);
    Task<PessoaListDto?> ObterAsync(Guid id, CancellationToken ct);
}
