using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

    public Task CriarVariosAsync(Guid pessoaId, IEnumerable<ContatoDto> contatos, CancellationToken ct)
    {
        if (contatos is null) return Task.CompletedTask;

        foreach (var contatoDto in contatos)
        {
            var contato = new Contato
            {
                Id = Guid.NewGuid(),
                PessoaId = pessoaId,
                Tipo = (contatoDto.Tipo ?? "").Trim().ToUpperInvariant(),
                Valor = (contatoDto.Valor ?? "").Trim(),
                Principal = contatoDto.Principal,
                DataCriacao = DateTime.UtcNow
            };

            _context.Set<Contato>().Add(contato);
        }

        return Task.CompletedTask; // ✅ sem SaveChanges
    }
}

