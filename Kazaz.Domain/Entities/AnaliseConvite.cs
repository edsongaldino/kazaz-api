using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public enum ResultadoAnaliseConvite
{
    Aprovado = 1,
    Reprovado = 2,
    CorrecaoSolicitada = 3
}

public class AnaliseConvite
{
    public Guid Id { get; set; }

    public Guid ConviteId { get; set; }
    public ConviteCadastroContrato Convite { get; set; } = null!;

    public ResultadoAnaliseConvite Resultado { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public string? Comentario { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
