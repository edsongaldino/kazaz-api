using System;
using System.Collections.Generic;
using Kazaz.Domain.Interfaces;

namespace Kazaz.Domain.Entities;

public sealed class Imovel : IMultiTenant
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = default!;
    public string? Titulo { get; set; }
    public FinalidadeImovel Finalidade { get; set; }
    public StatusImovel Status { get; set; }
    public Guid EnderecoId { get; set; }
    public Endereco Endereco { get; set; } = default!;

    public string? Observacoes { get; set; }

    public Guid TipoImovelId { get; set; }
    public TipoImovel TipoImovel { get; set; } = null!;

    public ICollection<ImovelCaracteristica> Caracteristicas { get; set; } = new List<ImovelCaracteristica>();

    public ICollection<Foto> Fotos { get; set; } = new List<Foto>();
    public ICollection<ImovelDocumento> Documentos { get; set; } = new List<ImovelDocumento>();

    /// <summary>
    /// Proprietários vinculados diretamente ao imóvel (independente de contratos).
    /// </summary>
    public ICollection<ImovelProprietario> Proprietarios { get; set; } = new List<ImovelProprietario>();

    public Guid? ImobiliariaId { get; set; }
    public Imobiliaria? Imobiliaria { get; set; }
}
