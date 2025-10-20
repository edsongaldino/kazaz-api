using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IAnexoDocumentoService
{
    Task<Guid> AnexarParaPessoaAsync(AnexarDocumentoPessoaDto dto, CancellationToken ct);
    Task<Guid> AnexarParaImovelAsync(AnexarDocumentoImovelDto dto, CancellationToken ct);
    Task RemoverPessoaAsync(Guid pessoaId, Guid tipoDocumentoId, CancellationToken ct);
    Task RemoverImovelAsync(Guid imovelId, Guid tipoDocumentoId, CancellationToken ct);
}
