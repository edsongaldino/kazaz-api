using System;
using System.Collections.Generic;
using Kazaz.Domain.Interfaces;

namespace Kazaz.Domain.Entities;

public class Colaborador : IMultiTenant
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Cpf { get; set; } = default!;
    public CargoColaborador Cargo { get; set; }
    public string Email { get; set; } = default!;
    public string? Telefone { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime? DataAdmissao { get; set; }

    public Guid? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public ICollection<ColaboradorDocumento> Documentos { get; set; } = new List<ColaboradorDocumento>();

    public Guid? ImobiliariaId { get; set; }
    public Imobiliaria? Imobiliaria { get; set; }
}
