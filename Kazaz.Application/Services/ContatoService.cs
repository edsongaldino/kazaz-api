using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Services;

public class ContatoService : IContatoService
{
	private readonly ApplicationDbContext _context;

	public ContatoService(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task CriarVariosAsync(Guid pessoaId, IEnumerable<ContatoCreateDto> contatos, CancellationToken ct)
	{
		if (contatos is null) return;

		foreach (var contatoDto in contatos)
		{
			var contato = new Contato
			{
				Id = Guid.NewGuid(),
				PessoaId = pessoaId,
				Tipo = contatoDto.Tipo.Trim().ToUpperInvariant(),
				Valor = contatoDto.Valor.Trim(),
				Principal = contatoDto.Principal,
				DataCriacao = DateTime.UtcNow
			};

			await _context.Set<Contato>().AddAsync(contato, ct);
		}

		await _context.SaveChangesAsync(ct);
	}
}

