using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/pessoas")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaFisicaService _pessoaFisicaService;
    private readonly IPessoaJuridicaService _pessoaJuridicaService;
    private readonly IEnderecoService _enderecoService;

    public PessoasController(
        IPessoaFisicaService pessoaFisicaService,
        IPessoaJuridicaService pessoaJuridicaService,
        IEnderecoService enderecoService)
    {
        _pessoaFisicaService = pessoaFisicaService;
        _pessoaJuridicaService = pessoaJuridicaService;
        _enderecoService = enderecoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? termo = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _pessoaFisicaService.ListarAsync(page, pageSize, termo, ct);
        return Ok(new { page, pageSize, total, items });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var dto = await _pessoaFisicaService.ObterAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    // 👇 FALTAVA O VERBO: resolve o erro do Swagger/404
    [HttpPost("")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarAsync([FromBody] PessoaCreateDto dto, CancellationToken ct)
    {
        if (dto is null) return BadRequest("Payload inválido.");

        var tipo = (dto.Tipo ?? string.Empty).Trim().ToUpperInvariant();
        if (tipo != "FISICA" && tipo != "JURIDICA")
            return BadRequest("Tipo deve ser 'FISICA' ou 'JURIDICA'.");

        if (dto.Endereco is null)
            return BadRequest("Endereço é obrigatório.");

        if (dto.Endereco.CidadeId == Guid.Empty)
            return BadRequest("Endereco.CidadeId é obrigatório.");

        // 1) Cria Endereço (usa seu EnderecoCreateDto com initializer)
        var endReq = new EnderecoCreateDto
        {
            Cep = Limpar(dto.Endereco.Cep),
            Logradouro = dto.Endereco.Logradouro?.Trim() ?? string.Empty,
            Numero = dto.Endereco.Numero?.Trim() ?? string.Empty,
            Complemento = string.IsNullOrWhiteSpace(dto.Endereco.Complemento) ? null : dto.Endereco.Complemento.Trim(),
            Bairro = dto.Endereco.Bairro?.Trim() ?? string.Empty,
            CidadeId = dto.Endereco.CidadeId
        };

        // Se seu service aceitar CancellationToken, passe 'ct'
        var endereco = await _enderecoService.CriarAsync(endReq /*, ct*/);

        // 2) Cria Pessoa PF ou PJ
        Guid pessoaId;
        if (tipo == "FISICA")
        {
            var pfReq = new PessoaFisicaCreateDto(
                Nome: (dto.Nome ?? string.Empty).Trim(),
                Cpf: Limpar(dto.Documento).PadLeft(11, '0'),
                Nascimento: dto.Nascimento,
                EnderecoId: endereco.Id
            );

            pessoaId = await _pessoaFisicaService.CriarAsync(pfReq, ct);
        }
        else
        {
            // Se Nome (fantasia) for realmente opcional e seu DTO não aceita null,
            // mande string.Empty ou mude a assinatura para string? Nome
            var pjReq = new PessoaJuridicaCreateDto(
                Nome: string.IsNullOrWhiteSpace(dto.Nome) ? string.Empty : dto.Nome!.Trim(),
                RazaoSocial: (dto.RazaoSocial ?? string.Empty).Trim(),
                Cnpj: Limpar(dto.Documento).PadLeft(14, '0'),
                EnderecoId: endereco.Id
            );

            pessoaId = await _pessoaJuridicaService.CriarAsync(pjReq, ct);
        }

        return Created($"/api/pessoas/{pessoaId}", new { id = pessoaId });
    }

    private static string Limpar(string? s)
        => new string((s ?? string.Empty).Where(char.IsDigit).ToArray());
}
