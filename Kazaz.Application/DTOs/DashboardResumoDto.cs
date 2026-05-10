namespace Kazaz.Application.DTOs;

public class DashboardResumoDto
{
    public int TotalImoveis { get; set; }
    public int TotalClientes { get; set; }
    public int TotalContratos { get; set; }
    public int TotalConvites { get; set; }

    public int ImoveisAtivos { get; set; }
    public int ImoveisEmNegociacao { get; set; }
    public int ImoveisVendidos { get; set; }
    public int ImoveisAlugados { get; set; }

    public List<DashboardGraficoItemDto> ImoveisPorTipo { get; set; } = [];
    public List<DashboardGraficoItemDto> ImoveisPorFinalidade { get; set; } = [];
    public List<DashboardGraficoItemDto> ConvitesPorStatus { get; set; } = [];
}

public class DashboardGraficoItemDto
{
    public string Label { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}