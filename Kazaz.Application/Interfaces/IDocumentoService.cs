using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IDocumentoService
{
    Task<IReadOnlyList<DocumentoListDto>> ListarPorPessoaAsync(Guid pessoaId, CancellationToken ct);
    Task<IReadOnlyList<DocumentoListDto>> ListarPorImovelAsync(Guid imovelId, CancellationToken ct);
    Task<DocumentoListDto?> ObterAsync(Guid id, CancellationToken ct);
    Task<Guid> CriarAsync(DocumentoCreateDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, DocumentoUpdateDto dto, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
}
