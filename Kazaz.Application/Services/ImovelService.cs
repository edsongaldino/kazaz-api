using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Interfaces.Services;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class ImovelService : IImovelService
{
	private readonly ApplicationDbContext ctx;
	private readonly IEnderecoService _enderecoService;

	public ImovelService(ApplicationDbContext ctx, IEnderecoService enderecoService)
	{
		this.ctx = ctx;
		_enderecoService = enderecoService;
	}

	public async Task<(IReadOnlyList<ImovelListDto> Items, int Total)> ListarAsync(
		int page, int pageSize, string? termo, CancellationToken ct)
	{
		page = page < 1 ? 1 : page;
		pageSize = pageSize < 1 ? 10 : pageSize;

		var q = ctx.Set<Imovel>().AsNoTracking();

		if (!string.IsNullOrWhiteSpace(termo))
			q = q.Where(i => EF.Functions.ILike(i.Codigo, $"%{termo}%"));

		var total = await q.CountAsync(ct);

		var items = await q.OrderBy(i => i.Codigo)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.Select(i => new ImovelListDto(i.Id, i.Codigo, i.EnderecoId))
			.ToListAsync(ct);

		return (items, total);
	}

	public async Task<ImovelListDto?> ObterAsync(Guid id, CancellationToken ct)
	{
		var i = await ctx.Set<Imovel>()
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id, ct);

		return i is null ? null : new ImovelListDto(i.Id, i.Codigo, i.EnderecoId);
	}

	public async Task<Guid> CriarAsync(ImovelUpsertDto dto, CancellationToken ct)
	{
		if (dto is null) throw new ArgumentNullException(nameof(dto));
		if (string.IsNullOrWhiteSpace(dto.Codigo))
			throw new ArgumentException("Código é obrigatório.", nameof(dto));

		await using var tx = await ctx.Database.BeginTransactionAsync(ct);

		var codigo = dto.Codigo.Trim();

		// valida código único
		var codigoExiste = await ctx.Set<Imovel>().AnyAsync(x => x.Codigo == codigo, ct);
		if (codigoExiste) throw new InvalidOperationException("Já existe um imóvel com este código.");

		// 1) Criar endereço (DTO sempre vem com Endereco)
		var endResp = await _enderecoService.CriarAsync(dto.Endereco);
		var enderecoId = endResp.Id;

		// 2) Criar Imóvel
		var ent = new Imovel
		{
			Id = Guid.NewGuid(),
			Codigo = codigo,
			Titulo = dto.Titulo?.Trim(),
			Finalidade = dto.Finalidade,
			Status = dto.Status,
			TipoImovelId = dto.TipoImovelId,
			EnderecoId = enderecoId,
			Observacoes = dto.Observacoes,
		};

		// 3) Sync coleções antes de salvar (como Id já é Guid.NewGuid(), funciona)
		SyncCaracteristicas(ent, dto.Caracteristicas);
		SyncVinculos(ent, dto.Vinculos);

		ctx.Add(ent);
		await ctx.SaveChangesAsync(ct);

		await tx.CommitAsync(ct);
		return ent.Id;
	}

	public async Task AtualizarAsync(Guid id, ImovelUpsertDto dto, CancellationToken ct)
	{
		if (dto is null) throw new ArgumentNullException(nameof(dto));
		if (string.IsNullOrWhiteSpace(dto.Codigo))
			throw new ArgumentException("Código é obrigatório.", nameof(dto));

		await using var tx = await ctx.Database.BeginTransactionAsync(ct);

		// ⚠️ importante: incluir coleções para sync
		var ent = await ctx.Set<Imovel>()
			.Include(x => x.Caracteristicas)
			.Include(x => x.Vinculos)
			.FirstOrDefaultAsync(x => x.Id == id, ct)
			?? throw new KeyNotFoundException("Imóvel não encontrado.");

		var codigo = dto.Codigo.Trim();

		// valida código único ignorando o próprio
		var codigoExiste = await ctx.Set<Imovel>()
			.AnyAsync(x => x.Codigo == codigo && x.Id != id, ct);

		if (codigoExiste) throw new InvalidOperationException("Já existe um imóvel com este código.");

		// 1) Atualizar endereço vinculado ao imóvel
		var endUpdate = new EnderecoUpdateDto
		{
			Id = ent.EnderecoId,
			Cep = dto.Endereco.Cep,
			Logradouro = dto.Endereco.Logradouro,
			Numero = dto.Endereco.Numero,
			Complemento = dto.Endereco.Complemento,
			Bairro = dto.Endereco.Bairro,
			CidadeId = dto.Endereco.CidadeId
		};

		await _enderecoService.AtualizarAsync(endUpdate);

		// 2) Atualizar dados do imóvel
		ent.Codigo = codigo;
		ent.Titulo = dto.Titulo?.Trim();
		ent.Finalidade = dto.Finalidade;
		ent.Status = dto.Status;
		ent.TipoImovelId = dto.TipoImovelId;
		ent.Observacoes = dto.Observacoes;

		// 3) Sync coleções
		SyncCaracteristicas(ent, dto.Caracteristicas);
		SyncVinculos(ent, dto.Vinculos);

		await ctx.SaveChangesAsync(ct);
		await tx.CommitAsync(ct);
	}

	public async Task RemoverAsync(Guid id, CancellationToken ct)
	{
		var ent = await ctx.Set<Imovel>().FirstOrDefaultAsync(x => x.Id == id, ct);
		if (ent is null) return;

		ctx.Remove(ent);
		await ctx.SaveChangesAsync(ct);
	}

	// ----------------- Sync helpers -----------------

	private static void SyncCaracteristicas(Imovel imovel, List<ImovelCaracteristicaUpsertDto> incoming)
	{
		incoming ??= new();

		// garante coleção
		imovel.Caracteristicas ??= new List<ImovelCaracteristica>(); // se for ICollection, ok também

		// remove as que não vieram
		var incomingIds = incoming.Select(x => x.CaracteristicaId).ToHashSet();

		var toRemove = imovel.Caracteristicas
			.Where(x => !incomingIds.Contains(x.CaracteristicaId))
			.ToList();

		foreach (var item in toRemove)
			imovel.Caracteristicas.Remove(item);

		// add/update
		foreach (var dto in incoming)
		{
			var existing = imovel.Caracteristicas.FirstOrDefault(x => x.CaracteristicaId == dto.CaracteristicaId);

			if (existing is null)
			{
				imovel.Caracteristicas.Add(new ImovelCaracteristica
				{
					Id = Guid.NewGuid(),
					ImovelId = imovel.Id,
					CaracteristicaId = dto.CaracteristicaId,
					ValorBool = dto.ValorBool,
					ValorInt = dto.ValorInt,
					ValorDecimal = dto.ValorDecimal,
					ValorTexto = dto.ValorTexto,
					ValorData = dto.ValorData,
					Observacao = dto.Observacao
				});
			}
			else
			{
				existing.ValorBool = dto.ValorBool;
				existing.ValorInt = dto.ValorInt;
				existing.ValorDecimal = dto.ValorDecimal;
				existing.ValorTexto = dto.ValorTexto;
				existing.ValorData = dto.ValorData;
				existing.Observacao = dto.Observacao;
			}
		}
	}


	private static void SyncVinculos(Imovel imovel, List<VinculoPessoaImovelUpsertDto> incoming)
	{
		incoming ??= new();

		imovel.Vinculos ??= new List<VinculoPessoaImovel>();

		var incomingKeys = incoming
			.Select(x => (x.PessoaId, x.PerfilVinculoImovelId))
			.ToHashSet();

		var toRemove = imovel.Vinculos
			.Where(v => !incomingKeys.Contains((v.PessoaId, v.PerfilVinculoImovelId)))
			.ToList();

		foreach (var item in toRemove)
			imovel.Vinculos.Remove(item);

		foreach (var dto in incoming)
		{
			var exists = imovel.Vinculos.Any(v =>
				v.PessoaId == dto.PessoaId &&
				v.PerfilVinculoImovelId == dto.PerfilVinculoImovelId);

			if (!exists)
			{
				imovel.Vinculos.Add(new VinculoPessoaImovel
				{
					Id = Guid.NewGuid(),
					ImovelId = imovel.Id,
					PessoaId = dto.PessoaId,
					PerfilVinculoImovelId = dto.PerfilVinculoImovelId
				});
			}
		}
	}
}
