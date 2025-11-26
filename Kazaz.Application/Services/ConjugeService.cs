using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Kazaz.SharedKernel.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Services;

public class ConjugeService : IConjugeService
{
	private readonly ApplicationDbContext _context;

	public ConjugeService(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task CriarOuAtualizarAsync(Guid pessoaId, ConjugeCreateDto dto, CancellationToken ct)
	{
		var existente = await _context.Set<Conjuge>()
			.FirstOrDefaultAsync(c => c.PessoaId == pessoaId, ct);

		if (existente is null)
		{
			var entity = new Conjuge
			{
				Id = Guid.NewGuid(),
				PessoaId = pessoaId,
				Nome = dto.Nome.Trim(),
				Cpf = DocumentoHelper.Limpar(dto.Cpf).PadLeft(11, '0'),
				Rg = string.IsNullOrWhiteSpace(dto.Rg) ? null : dto.Rg.Trim(),
				OrgaoExpedidor = string.IsNullOrWhiteSpace(dto.OrgaoExpedidor) ? null : dto.OrgaoExpedidor.Trim(),
				DataNascimento = dto.DataNascimento,
				Telefone = string.IsNullOrWhiteSpace(dto.Telefone) ? null : dto.Telefone.Trim(),
				Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
				DataCriacao = DateTime.UtcNow
			};

			await _context.Set<Conjuge>().AddAsync(entity, ct);
		}
		else
		{
			existente.Nome = dto.Nome.Trim();
			existente.Cpf = DocumentoHelper.Limpar(dto.Cpf).PadLeft(11, '0');
			existente.Rg = string.IsNullOrWhiteSpace(dto.Rg) ? null : dto.Rg.Trim();
			existente.OrgaoExpedidor = string.IsNullOrWhiteSpace(dto.OrgaoExpedidor) ? null : dto.OrgaoExpedidor.Trim();
			existente.DataNascimento = dto.DataNascimento;
			existente.Telefone = string.IsNullOrWhiteSpace(dto.Telefone) ? null : dto.Telefone.Trim();
			existente.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
			existente.DataAtualizacao = DateTime.UtcNow;
		}

		await _context.SaveChangesAsync(ct);
	}
}

