using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/uploads")]
public class UploadsController : ControllerBase
{
    private readonly IFileStorage _storage;

    public UploadsController(IFileStorage storage)
    {
        _storage = storage;
    }

    /// <summary>
    /// Salva o arquivo no disco e devolve metadados.
    /// O "Caminho" retornado deve ser gravado em Documento.Caminho.
    /// NÃO cria registro no banco.
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<UploadArquivoResponse>> Upload(
        IFormFile file,
        [FromQuery] string? folder,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Arquivo inválido." });

        var sub = string.IsNullOrWhiteSpace(folder) ? "docs" : folder.Trim();

        var saved = await _storage.SaveAsync(file, sub, ct);

        return Ok(new UploadArquivoResponse(
            Nome: saved.NomeOriginal,
            Caminho: saved.Caminho,
            ContentType: saved.ContentType,
            TamanhoBytes: saved.TamanhoBytes
        ));
    }

    // (Opcional) baixar arquivo pelo caminho relativo
    // ⚠️ Recomendo proteger isso com auth depois.
    [HttpGet]
    public IActionResult Download([FromQuery] string caminho)
    {
        try
        {
            var full = _storage.GetPhysicalPath(caminho);
            if (!System.IO.File.Exists(full))
                return NotFound();

            return PhysicalFile(full, "application/octet-stream", enableRangeProcessing: true);
        }
        catch
        {
            return BadRequest(new { message = "Caminho inválido." });
        }
    }
}
