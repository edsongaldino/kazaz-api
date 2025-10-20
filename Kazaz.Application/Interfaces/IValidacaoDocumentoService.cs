using Kazaz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IValidacaoDocumentoService
{
    Task<List<TipoDocumento>> FaltantesPessoaAsync(Guid pessoaId, CancellationToken ct);
    Task<List<TipoDocumento>> FaltantesImovelAsync(Guid imovelId, CancellationToken ct);
}
