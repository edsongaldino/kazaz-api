using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IRegrasDocumentoCadastroService
{
    Task<List<RegraDocumentoCadastroResponse>> ListarTodasAsync(CancellationToken ct);
    Task<Guid> CriarAsync(CriarRegraDocumentoCadastroRequest req, CancellationToken ct);
    Task AtualizarAsync(Guid id, AtualizarRegraDocumentoCadastroRequest req, CancellationToken ct);
    Task ExcluirAsync(Guid id, CancellationToken ct);
}
