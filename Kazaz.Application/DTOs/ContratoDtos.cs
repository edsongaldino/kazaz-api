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
    string? CodigoImovel,
    string? TituloImovel,
    string? TipoImovelNome,
    DateOnly InicioVigencia,
    DateOnly? FimVigencia,
    DateTime CriadoEm,
    List<ContratoParteResponse> Partes
);

public class ListarContratosQuery
{
    public Guid? ImovelId { get; set; }
    public Guid? TipoImovelId { get; set; }

    public TipoContrato? Tipo { get; set; }
    public StatusContrato? Status { get; set; }

    public string? Contrato { get; set; } // número
    public string? Imovel { get; set; }   // código/título
    public string? DocumentoParte { get; set; }

    public DateOnly? VigenciaDe { get; set; }
    public DateOnly? VigenciaAte { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
