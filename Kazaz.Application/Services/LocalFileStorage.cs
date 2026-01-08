using Kazaz.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Kazaz.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    public string RootPath { get; }

    public LocalFileStorage(IConfiguration cfg)
    {
        RootPath = cfg["Storage:LocalPath"] ?? throw new InvalidOperationException("Storage:LocalPath não configurado.");
        Directory.CreateDirectory(RootPath);
    }

    public async Task<(string Caminho, long TamanhoBytes, string? ContentType, string NomeOriginal)> SaveAsync(
        IFormFile file,
        string subPasta,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            throw new InvalidOperationException("Arquivo inválido.");

        subPasta = SanitizeFolder(subPasta);

        var relDir = $"{subPasta}/{DateTime.UtcNow:yyyy/MM/dd}";
        var nomeOriginal = Path.GetFileName(file.FileName);
        var ext = Path.GetExtension(nomeOriginal);

        var storedName = $"{Guid.NewGuid():N}{ext}";
        var caminho = $"{relDir}/{storedName}"; // RELATIVO (vai para Documento.Caminho)

        var fullDir = Path.Combine(RootPath, relDir.Replace("/", Path.DirectorySeparatorChar.ToString()));
        Directory.CreateDirectory(fullDir);

        var fullFile = Path.Combine(fullDir, storedName);

        await using (var fs = new FileStream(fullFile, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await file.CopyToAsync(fs, ct);
        }

        return (caminho, file.Length, file.ContentType, nomeOriginal);
    }

    public Task DeleteAsync(string caminho, CancellationToken ct)
    {
        caminho = NormalizeAndValidateRelativePath(caminho);
        if (caminho is null) return Task.CompletedTask;

        var full = GetPhysicalPath(caminho);
        if (File.Exists(full))
            File.Delete(full);

        return Task.CompletedTask;
    }

    public string GetPhysicalPath(string caminho)
    {
        caminho = NormalizeAndValidateRelativePath(caminho)
            ?? throw new InvalidOperationException("Caminho inválido.");

        return Path.Combine(RootPath, caminho.Replace("/", Path.DirectorySeparatorChar.ToString()));
    }

    private static string? NormalizeAndValidateRelativePath(string caminho)
    {
        caminho = (caminho ?? "").Trim().Replace("\\", "/");
        if (string.IsNullOrWhiteSpace(caminho)) return null;

        // bloqueia path traversal
        if (caminho.Contains("..")) return null;
        if (caminho.StartsWith("/")) caminho = caminho.TrimStart('/');

        return caminho;
    }

    private static string SanitizeFolder(string folder)
    {
        folder = (folder ?? "").Trim().Replace("\\", "/");
        folder = folder.Replace("..", "").Trim('/');
        if (string.IsNullOrWhiteSpace(folder)) return "docs";

        var safe = new string(folder.Select(ch =>
            char.IsLetterOrDigit(ch) || ch == '/' || ch == '-' || ch == '_' ? ch : '-'
        ).ToArray());

        safe = safe.Trim('/');
        return string.IsNullOrWhiteSpace(safe) ? "docs" : safe;
    }
}
