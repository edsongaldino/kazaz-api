using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public record DadosComplementaresCreateDto(
    string? Profissao,
    string? Escolaridade,
    decimal? RendaMensal,
    string? Observacoes
);


