using System;

namespace Kazaz.Domain.Entities;

public class Imobiliaria
{
    public Guid Id { get; set; }
    public string RazaoSocial { get; set; } = default!;
    public string NomeFantasia { get; set; } = default!;
    public string Cnpj { get; set; } = default!;
    public string Creci { get; set; } = default!;
    public DateTime? DataFundacao { get; set; }
    public string? LogoUrl { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }

    public Guid? EnderecoId { get; set; }
    public Endereco? Endereco { get; set; }
}
