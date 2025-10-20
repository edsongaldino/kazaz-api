using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class ImovelDocumento
{
    public Guid Id { get; set; }
    public Guid ImovelId { get; set; }
    public Guid TipoDocumentoId { get; set; }
    public Guid DocumentoId { get; set; }
    public DateTime DataAnexo { get; set; } = DateTime.UtcNow;
    public string? Observacao { get; set; }

    public Imovel Imovel { get; set; } = default!;
    public TipoDocumento Tipo { get; set; } = default!;
    public Documento Documento { get; set; } = default!;
}
