using Kazaz.Domain.Entities;

public class Endereco
{
    public Guid Id { get; set; }

    public string Cep { get; set; } = null!;
    public string Logradouro { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = null!;
    public Guid? CidadeId { get; set; }
    public Cidade? Cidade { get; set; }

}


public class EnderecoRequest
{
    public string Cep { get; set; } = null!;
    public string Logradouro { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = null!;
    public Guid? CidadeId { get; set; }

    public Endereco ToEntity()
    {
        return new Endereco
        {
            Id = Guid.NewGuid(),
            Cep = Cep,
            Logradouro = Logradouro,
            Numero = Numero,
            Complemento = Complemento,
            Bairro = Bairro,
            CidadeId = CidadeId
        };
    }
}

public record EnderecoListDto(
    string Cep,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string? CidadeNome,
    string? EstadoSigla
);
