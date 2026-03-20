using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Interfaces.Services;
using Kazaz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/pessoas")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaFisicaService _pessoaFisicaService;
    private readonly IPessoaJuridicaService _pessoaJuridicaService;
    private readonly IEnderecoService _enderecoService;
    private readonly IPessoaService _pessoaService;
	private readonly IContatoService _contatoService;
	private readonly IDadosComplementaresService _dadosComplementaresService;
	private readonly IConjugeService _conjugeService;

	public PessoasController(
        IPessoaFisicaService pessoaFisicaService,
        IPessoaService pessoaService,
        IPessoaJuridicaService pessoaJuridicaService,
        IEnderecoService enderecoService,
		IContatoService contatoService,
		IConjugeService conjugeService,
		IDadosComplementaresService dadosComplementaresService
		)
    {
        _pessoaService = pessoaService;
        _pessoaFisicaService = pessoaFisicaService;
        _pessoaJuridicaService = pessoaJuridicaService;
        _enderecoService = enderecoService;
		_contatoService = contatoService;
		_dadosComplementaresService = dadosComplementaresService;
        _conjugeService = conjugeService;

	}

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? termo = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _pessoaService.ListarAsync(page, pageSize, termo, ct);
        return Ok(new { page, pageSize, total, items });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var dto = await _pessoaService.ObterAsync(id, ct);
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

        if (dto.PessoaId.HasValue)
        {
            if (dto.PessoaId.Value == Guid.Empty)
                return BadRequest("PessoaId inválido.");

            var existente = await _pessoaService.ObterAsync(dto.PessoaId.Value, ct);
            if (existente is null)
                return BadRequest("Pessoa não encontrada para vínculo.");

            // opcional: atualizar origem se veio
            //if (dto.OrigemId is not null)
                //await _pessoaService.AtualizarOrigemAsync(dto.PessoaId.Value, dto.OrigemId, ct);

            // opcional: se quiser permitir incluir contatos/dados complementares/conjuge mesmo no vínculo:
            if (dto.Contatos is not null && dto.Contatos.Any())
                await _contatoService.CriarVariosAsync(dto.PessoaId.Value, dto.Contatos, ct);

            if (dto.DadosComplementares is not null)
                await _dadosComplementaresService.CriarOuAtualizarAsync(dto.PessoaId.Value, dto.DadosComplementares, ct);

            if (dto.Conjuge is not null)
                await _conjugeService.CriarOuAtualizarAsync(dto.PessoaId.Value, dto.Conjuge, ct);

            return Created($"/api/pessoas/{dto.PessoaId.Value}", new { id = dto.PessoaId.Value });
        }

        var tipo = (dto.Tipo ?? string.Empty).Trim().ToUpperInvariant();
        if (tipo != "PF" && tipo != "PJ")
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

        // 2) ✅ Cria PRIMEIRO a Pessoa base (linha em pessoas)
        //    - Nome vem do bloco PF/PJ (igual você já usa)
        string nomeBase;
        if (tipo == "PF")
        {
            nomeBase = (dto.DadosPessoaFisica?.Nome ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nomeBase))
                return BadRequest("DadosPessoaFisica.Nome é obrigatório.");
        }
        else
        {
            // escolha a regra: geralmente PJ usa RazaoSocial como nome base
            nomeBase = (dto.DadosPessoaJuridica?.RazaoSocial ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nomeBase))
                return BadRequest("DadosPessoaJuridica.RazaoSocial é obrigatório.");
        }

        var pessoaId = await _pessoaService.CriarBaseAsync(
            nome: nomeBase,
            enderecoId: endereco.Id,
            origemId: dto.OrigemId,
            ct: ct
        );

        if (tipo == "PF")
        {
            var pfReq = new DadosPessoaFisicaDto(
                Nome: (dto.DadosPessoaFisica.Nome ?? string.Empty).Trim(),
                Rg: (dto.DadosPessoaFisica.Rg ?? string.Empty).Trim(),
                OrgaoExpedidor: (dto.DadosPessoaFisica.OrgaoExpedidor ?? string.Empty).Trim(),
                Cpf: Limpar(dto.Documento).PadLeft(11, '0'),
                DataNascimento: dto.DadosPessoaFisica.DataNascimento,
                EstadoCivil: dto.DadosPessoaFisica.EstadoCivil,
                Nacionalidade: dto.DadosPessoaFisica.Nacionalidade
            );

            await _pessoaFisicaService.CriarAsync(pessoaId,pfReq, ct);
        }
        else
        {
            // Se Nome (fantasia) for realmente opcional e seu DTO não aceita null,
            // mande string.Empty ou mude a assinatura para string? Nome
            var pjReq = new DadosPessoaJuridicaDto(
                NomeFantasia: (dto.DadosPessoaJuridica.NomeFantasia ?? string.Empty).Trim(),
                RazaoSocial: (dto.DadosPessoaJuridica.RazaoSocial ?? string.Empty).Trim(),
                DataAbertura: (dto.DadosPessoaJuridica.DataAbertura),
                Cnpj: Limpar(dto.Documento).PadLeft(14, '0'),
                InscricaoEstadual: dto.DadosPessoaJuridica.InscricaoEstadual
            );

            await _pessoaJuridicaService.CriarAsync(pessoaId, pjReq, ct);
        }

        if (dto.Contatos is not null && dto.Contatos.Any())
		{
			await _contatoService.CriarVariosAsync(pessoaId, dto.Contatos, ct);
		}

		if (dto.DadosComplementares is not null)
		{
			await _dadosComplementaresService.CriarOuAtualizarAsync(
				pessoaId,
				dto.DadosComplementares,
				ct
			);
		}

		if (dto.Conjuge is not null)
		{
			await _conjugeService.CriarOuAtualizarAsync(
				pessoaId,
				dto.Conjuge,
				ct
			);
		}

		return Created($"/api/pessoas/{pessoaId}", new { id = pessoaId });
    }

    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditarAsync([FromRoute] Guid id, [FromBody] PessoaUpdateDto dto, CancellationToken ct)
    {
        if (id == Guid.Empty) return BadRequest("Id inválido.");
        if (dto is null) return BadRequest("Payload inválido.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // Endereço: upsert opcional
        Guid? enderecoId = null;
        if (dto.Endereco is not null)
        {
            if (dto.Endereco is not null && dto.Endereco.Id != Guid.Empty)
            {
                var eUpd = new EnderecoUpdateDto
                {
                    Id = dto.Endereco.Id,
                    Cep = Limpar(dto.Endereco.Cep),
                    Logradouro = dto.Endereco.Logradouro?.Trim(),
                    Numero = dto.Endereco.Numero?.Trim(),
                    Complemento = string.IsNullOrWhiteSpace(dto.Endereco.Complemento) ? null : dto.Endereco.Complemento!.Trim(),
                    Bairro = dto.Endereco.Bairro?.Trim(),
                    CidadeId = dto.Endereco.CidadeId
                };
                await _enderecoService.AtualizarAsync(eUpd);
                enderecoId = eUpd.Id;
            }
            else
            {
                if (dto.Endereco.CidadeId == Guid.Empty)
                    return BadRequest("Endereco.CidadeId é obrigatório.");

                var eReq = new EnderecoCreateDto
                {
                    Cep = Limpar(dto.Endereco.Cep ?? ""),
                    Logradouro = dto.Endereco.Logradouro?.Trim() ?? string.Empty,
                    Numero = dto.Endereco.Numero?.Trim() ?? string.Empty,
                    Complemento = string.IsNullOrWhiteSpace(dto.Endereco.Complemento) ? null : dto.Endereco.Complemento!.Trim(),
                    Bairro = dto.Endereco.Bairro?.Trim() ?? string.Empty,
                    CidadeId = dto.Endereco.CidadeId
                };

                var novo = await _enderecoService.CriarAsync(eReq);
                enderecoId = novo.Id;
            }
        }

        if (dto.Tipo == "PF")
        {
            var pf = new PessoaFisicaUpdateDto(
                Nome: dto.DadosPessoaFisica.Nome?.Trim(),
                Rg: (dto.DadosPessoaFisica.Rg ?? string.Empty).Trim(),
                OrgaoExpedidor: (dto.DadosPessoaFisica.OrgaoExpedidor ?? string.Empty).Trim(),
                Nacionalidade: (dto.DadosPessoaFisica.Nacionalidade ?? string.Empty).Trim(),
                Cpf: string.IsNullOrWhiteSpace(dto.Documento) ? null : Limpar(dto.Documento).PadLeft(11, '0'),
                DataNascimento: dto.DadosPessoaFisica.DataNascimento,
                EstadoCivil: dto.DadosPessoaFisica.EstadoCivil
            );
            await _pessoaFisicaService.AtualizarAsync(id, pf, ct);
        }
        else // "PJ"
        {
            var pj = new PessoaJuridicaUpdateDto(
                RazaoSocial: dto.DadosPessoaJuridica.RazaoSocial?.Trim(),
                NomeFantasia: dto.DadosPessoaJuridica.NomeFantasia?.Trim(),
                DataAbertura: dto.DadosPessoaJuridica.DataAbertura,
                Cnpj: string.IsNullOrWhiteSpace(dto.Documento) ? null : Limpar(dto.Documento).PadLeft(14, '0'),
                InscricaoEstadual: dto.DadosPessoaJuridica.InscricaoEstadual?.Trim()
            );
            await _pessoaJuridicaService.AtualizarAsync(id, pj, ct);
        }

        await _pessoaService.AtualizarAsync(id, dto, enderecoId, ct);

        return NoContent();
    }

    [HttpGet("por-documento/{documento}")]
    public async Task<IActionResult> ObterPorDocumento(string documento, CancellationToken ct)
    {
        var digits = Limpar(documento);

        if (digits.Length != 11 && digits.Length != 14)
            return BadRequest("Documento inválido.");

        var dto = await _pessoaService.ObterPorDocumentoAsync(digits, ct);
        return dto is null ? NotFound() : Ok(dto);
    }


    private static string Limpar(string? s)
        => new string((s ?? string.Empty).Where(char.IsDigit).ToArray());
}
