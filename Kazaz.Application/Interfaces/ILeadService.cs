using System;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;
using Kazaz.SharedKernel;

namespace Kazaz.Application.Interfaces;

public interface ILeadService
{
    Task<LeadResponseDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<LeadsPagedResponse> SearchAsync(LeadSearchFilterDto filter, CancellationToken ct);
    Task<Guid> CreateAsync(LeadCreateDto dto, CancellationToken ct);
    Task UpdateAsync(Guid id, LeadUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<Guid> ConvertToClientAsync(Guid leadId, ConvertLeadRequest request, CancellationToken ct);
}
