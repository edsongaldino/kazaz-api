using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public record ContatoDto(
    string Tipo,     // "EMAIL", "TELEFONE", "WHATSAPP" etc.
    string Valor,    // endereço de email, número de telefone etc.
    bool Principal   // se é o contato principal
);

public sealed record ContatoUpdateDto(
    string Tipo,
    string Valor,
    bool Principal
);
