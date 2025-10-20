using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IVinculoImovelService
{
    Task VincularAsync(VincularPessoaImovelDto dto, CancellationToken ct);
    Task DesvincularAsync(Guid pessoaId, Guid imovelId, Guid? perfilVinculoImovelId, CancellationToken ct);
}
