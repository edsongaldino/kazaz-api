using System;

namespace Kazaz.Domain.Entities;

public class ColaboradorDocumento
{
    public Guid Id { get; set; }
    public Guid ColaboradorId { get; set; }
    public string Nome { get; set; } = default!; // e.g. "RG", "CPF"
    public Guid DocumentoId { get; set; }
    public DateTime DataAnexo { get; set; } = DateTime.UtcNow;

    public Colaborador Colaborador { get; set; } = default!;
    public Documento Documento { get; set; } = default!;
}
