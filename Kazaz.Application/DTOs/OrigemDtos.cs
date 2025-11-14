using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;


public record OrigemCreateDto(
    string Nome,
    string? Descricao
);

public record OrigemUpdateDto(
    string Nome,
    string? Descricao
);
public record OrigemResponseDto(
    Guid Id,
    string Nome,
    string? Descricao
);
