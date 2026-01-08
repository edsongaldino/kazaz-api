using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public record ContratoParteRequest(
    Guid PessoaId,
    int Papel,
    decimal? Percentual
);

public record CriarContratoRequest(
    int Tipo,
    Guid ImovelId,
    DateOnly InicioVigencia,
    DateOnly? FimVigencia,
    List<ContratoParteRequest> Partes
);

public record ContratoParteResponse(
    Guid PessoaId,
    string PessoaNome,
    int Papel,
    decimal? Percentual
);

public record ContratoResponse(
    Guid Id,
    string Numero,
    int Tipo,
    int Status,
    Guid ImovelId,
    DateOnly InicioVigencia,
    DateOnly? FimVigencia,
    DateTime CriadoEm,
    List<ContratoParteResponse> Partes
);
