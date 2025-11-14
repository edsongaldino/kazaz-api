using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IOrigemService
{
    Task<OrigemResponseDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PagedResult<OrigemResponseDto>> SearchAsync(string? q, int page, int pageSize, CancellationToken ct);
    Task<Guid> CreateAsync(OrigemCreateDto dto, CancellationToken ct);
    Task UpdateAsync(Guid id, OrigemUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
