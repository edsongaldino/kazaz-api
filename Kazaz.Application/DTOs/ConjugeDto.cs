using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public record ConjugeDto(
    string? Nome,
    string? Cpf,
    DateOnly? DataNascimento,
    string? Telefone,
    string? Email
);
