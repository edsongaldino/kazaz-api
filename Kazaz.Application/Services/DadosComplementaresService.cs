namespace Kazaz.Application.Services;

using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class DadosComplementaresService : IDadosComplementaresService
{
	private readonly ApplicationDbContext _context;

	public DadosComplementaresService(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task CriarOuAtualizarAsync(Guid pessoaId, DadosComplementaresDto dto, CancellationToken ct)
	{
		var existente = await _context.Set<DadosComplementares>()
			.FirstOrDefaultAsync(d => d.PessoaId == pessoaId, ct);

		if (existente is null)
		{
			var entity = new DadosComplementares
			{
				Id = Guid.NewGuid(),
				PessoaId = pessoaId,
				Profissao = string.IsNullOrWhiteSpace(dto.Profissao) ? null : dto.Profissao.Trim(),
				Escolaridade = string.IsNullOrWhiteSpace(dto.Escolaridade) ? null : dto.Escolaridade.Trim(),
				RendaMensal = dto.RendaMensal,
				Observacoes = string.IsNullOrWhiteSpace(dto.Observacoes) ? null : dto.Observacoes.Trim(),
				DataCriacao = DateTime.UtcNow
			};

			await _context.Set<DadosComplementares>().AddAsync(entity, ct);
		}
		else
		{
			existente.Profissao = string.IsNullOrWhiteSpace(dto.Profissao) ? null : dto.Profissao.Trim();
			existente.Escolaridade = string.IsNullOrWhiteSpace(dto.Escolaridade) ? null : dto.Escolaridade.Trim();
			existente.RendaMensal = dto.RendaMensal;
			existente.Observacoes = string.IsNullOrWhiteSpace(dto.Observacoes) ? null : dto.Observacoes.Trim();
			existente.DataAtualizacao = DateTime.UtcNow;
		}

		await _context.SaveChangesAsync(ct);
	}
}

