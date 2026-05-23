using System;

namespace Kazaz.Application.DTOs;

public record ContratoChecklistEntradaResponse(
    Guid ContratoId,
    DateOnly? AssinadoEm,
    string? SeguroIncendio,
    string? Chaves,
    string? Energia,
    string? Agua,
    string? Gas,
    string? Condominio,
    string? IptuGaragem,
    string? Iptu,
    DateOnly? VistoriaEntradaEm,
    string? Manutencao,
    string? ObservacoesFinais,
    string? BonusLocacao,
    DateOnly? DataPagamentoBonus,
    string? EtapasPersonalizadasJson
);

public record SalvarChecklistEntradaRequest(
    DateOnly? AssinadoEm,
    string? SeguroIncendio,
    string? Chaves,
    string? Energia,
    string? Agua,
    string? Gas,
    string? Condominio,
    string? IptuGaragem,
    string? Iptu,
    DateOnly? VistoriaEntradaEm,
    string? Manutencao,
    string? ObservacoesFinais,
    string? BonusLocacao,
    DateOnly? DataPagamentoBonus,
    string? EtapasPersonalizadasJson
);

public record ContratoChecklistSaidaResponse(
    Guid ContratoId,
    string? MotivoSaida,
    string? Aluguel,
    string? MultaContratual,
    DateOnly? AvisoSaidaEm,
    string? Chaves,
    string? AvisoProprietario,
    string? Energia,
    string? Gas,
    string? Agua,
    string? Condominio,
    string? Iptu,
    DateOnly? VistoriaSaidaEm,
    string? PinturaManutencao,
    string? ReativarImovelNoSite,
    string? CancelamentoSeguroFianca,
    string? EtapasPersonalizadasJson
);

public record SalvarChecklistSaidaRequest(
    string? MotivoSaida,
    string? Aluguel,
    string? MultaContratual,
    DateOnly? AvisoSaidaEm,
    string? Chaves,
    string? AvisoProprietario,
    string? Energia,
    string? Gas,
    string? Agua,
    string? Condominio,
    string? Iptu,
    DateOnly? VistoriaSaidaEm,
    string? PinturaManutencao,
    string? ReativarImovelNoSite,
    string? CancelamentoSeguroFianca,
    string? EtapasPersonalizadasJson
);

public record ChecklistEtapaGlobalResponse(
    Guid Id,
    string TipoChecklist,
    string Label,
    string TipoField,
    string Card
);

public record CriarChecklistEtapaGlobalRequest(
    string TipoChecklist,
    string Label,
    string TipoField,
    string Card
);
