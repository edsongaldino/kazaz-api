using Microsoft.AspNetCore.Http;

namespace Kazaz.Application.Interfaces.Storage;

public interface IFileStorage
{
    string RootPath { get; }

    Task<(string Caminho, long TamanhoBytes, string? ContentType, string NomeOriginal)> SaveAsync(
        IFormFile file,
        string subPasta,
        CancellationToken ct
    );

    Task DeleteAsync(string caminho, CancellationToken ct);

    string GetPhysicalPath(string caminho);
}
