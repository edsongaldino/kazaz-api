using Kazaz.Application.DTOs;
using Kazaz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface ITipoDocumentoService
{
    Task<Guid> CriarAsync(TipoDocumentoCreateDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, TipoDocumentoUpdateDto dto, CancellationToken ct);
    Task<List<TipoDocumento>> ListarPorAlvoAsync(AlvoDocumento alvo, bool? somenteObrigatorios, CancellationToken ct);
}
