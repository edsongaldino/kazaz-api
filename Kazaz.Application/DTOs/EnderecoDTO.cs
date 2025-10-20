using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kazaz.Application.DTOs
{
    public class EnderecoCreateDto
    {
        public string Cep { get; set; } = null!;
        public string Logradouro { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = null!;
        public Guid? CidadeId { get; set; }

    }

    public class EnderecoUpdateDto : EnderecoCreateDto
    {
        public Guid Id { get; set; }
    }

    public class EnderecoResponseDto
    {
        public Guid Id { get; set; }
        public Guid? CidadeId { get; set; }

        // básicos
        public string? Cep { get; set; }
        public string Logradouro { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = null!;
    }

    public record EstadoDto(Guid Id, string Nome, string Uf);
    public record CidadeDto(Guid Id, string Nome, string Ibge, Guid EstadoId, string Uf);
    public record CidadeSlimDto(Guid Id, string Nome);

}
