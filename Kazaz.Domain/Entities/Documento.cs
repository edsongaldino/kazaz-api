using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class Documento
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Caminho { get; set; }
    public string? ContentType { get; set; }
    public long? TamanhoBytes { get; set; }
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;
}
