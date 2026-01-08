using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public sealed record TipoImovelListDto(
    Guid Id,
    string Nome,
    bool Ativo,
    int Ordem,
    string? Categoria
);
