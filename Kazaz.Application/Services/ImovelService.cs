using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Interfaces.Services;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            .Select(i => new ImovelListDto(
                i.Id,
                i.Codigo,
                i.Titulo,
                i.Finalidade,
                i.Status,
                i.TipoImovel.Nome,
                new EnderecoListDto(
                    i.Endereco.Cep,
                    i.Endereco.Logradouro,
                    i.Endereco.Numero,
                    i.Endereco.Complemento,
                    i.Endereco.Bairro,
                    i.Endereco.Cidade != null ? i.Endereco.Cidade.Nome : null,
                    i.Endereco.Cidade != null && i.Endereco.Cidade.Estado != null
                        ? i.Endereco.Cidade.Estado.Uf
                        : null
                )
            ))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<ImovelDetailsDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        // 1) carrega o imóvel com o que é "próprio" dele
        var imovel = await ctx.Set<Imovel>()
            .AsNoTracking()
            .Include(i => i.TipoImovel)
            .Include(i => i.Endereco)
                .ThenInclude(e => e.Cidade)
            .Include(i => i.Caracteristicas)
                .ThenInclude(ic => ic.Caracteristica)
            .Include(i => i.Fotos)
            .Include(i => i.Documentos)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

        if (imovel is null) return null;

        // 2) carrega contratos relacionados ao imóvel (novo "vínculo")
        var contratos = await ctx.Set<Contrato>()
            .AsNoTracking()
            .Where(c => c.ImovelId == id)
            .Include(c => c.Partes)
                .ThenInclude(p => p.Pessoa)
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync(ct);

        var caracteristicas = imovel.Caracteristicas
            .OrderBy(x => x.Caracteristica.Ordem)
            .ThenBy(x => x.Caracteristica.Nome)
            .Select(x => new ImovelCaracteristicaDto(
                Id: x.Id,
                CaracteristicaId: x.CaracteristicaId,
                CaracteristicaNome: x.Caracteristica.Nome,
                TipoValor: x.Caracteristica.TipoValor,
                Unidade: x.Caracteristica.Unidade,
                Grupo: x.Caracteristica.Grupo,
                ValorBool: x.ValorBool,
                ValorInt: x.ValorInt,
                ValorDecimal: x.ValorDecimal,
                ValorTexto: x.ValorTexto,
                ValorData: x.ValorData,
                Observacao: x.Observacao
            ))
            .ToList();

        // ✅ Novo: Contratos no details do imóvel
        var contratosDto = contratos
            .Select(c => new ImovelContratoResumoDto(
                Id: c.Id,
                Numero: c.Numero,
                Tipo: c.Tipo,
                Status: c.Status,
                InicioVigencia: c.InicioVigencia,
                FimVigencia: c.FimVigencia,
                Partes: c.Partes
                    .OrderBy(p => p.Papel)
                    .ThenBy(p => p.Pessoa.Nome)
                    .Select(p => new ImovelContratoParteDto(
                        PessoaId: p.PessoaId,
                        PessoaNome: p.Pessoa.Nome,
                        Papel: p.Papel,
                        Percentual: p.Percentual
                    ))
                    .ToList()
            ))
            .ToList();

        var fotos = imovel.Fotos?
            .Select(f => new ImovelFotoDto(
                Id: f.Id,
                Url: f.Caminho,
                Ordem: f.Ordem,
                Principal: true
            ))
            .ToList()
            ?? new List<ImovelFotoDto>();

        var documentos = imovel.Documentos?
            .Select(d => new ImovelDocumentoDto(
                Id: d.Id,
                Nome: d.Observacao,
                Url: ""
            ))
            .ToList()
            ?? new List<ImovelDocumentoDto>();

        return new ImovelDetailsDto(
            Id: imovel.Id,
            Codigo: imovel.Codigo,
            Titulo: imovel.Titulo,
            Finalidade: imovel.Finalidade,
            Status: imovel.Status,
            TipoImovelId: imovel.TipoImovelId,
            TipoImovelNome: imovel.TipoImovel?.Nome ?? "",
            EnderecoId: imovel.EnderecoId,
            Observacoes: imovel.Observacoes,
            Endereco: imovel.Endereco,
            Caracteristicas: caracteristicas,
            Contratos: contratosDto,
            Fotos: fotos,
            Documentos: documentos
        );
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

        // 1) Criar endereço
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

        // 3) Sync coleções
        SyncCaracteristicas(ent, dto.Caracteristicas);

        // ❌ vínculos agora vêm do Contrato
        // Se o dto ainda tiver Vinculos, ignore ou bloqueie:
        // if (dto.Vinculos?.Count > 0) throw new InvalidOperationException("Vínculos do imóvel agora são definidos por contratos.");

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
        await ReplaceCaracteristicasAsync(ent.Id, dto.Caracteristicas, ct);

        // ❌ vínculos agora vêm do Contrato
        // if (dto.Vinculos?.Count > 0) throw new InvalidOperationException("Vínculos do imóvel agora são definidos por contratos.");

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
        imovel.Caracteristicas ??= new List<ImovelCaracteristica>();

        var mapIncoming = incoming
            .GroupBy(x => x.CaracteristicaId)
            .Select(g => g.First())
            .ToDictionary(x => x.CaracteristicaId);

        var toRemove = imovel.Caracteristicas
            .Where(x => !mapIncoming.ContainsKey(x.CaracteristicaId))
            .Where(x => x.ValorBool == null)
            .ToList();

        foreach (var item in toRemove)
            imovel.Caracteristicas.Remove(item);

        foreach (var dto in mapIncoming.Values)
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

    private async Task ReplaceCaracteristicasAsync(Guid imovelId, List<ImovelCaracteristicaUpsertDto>? incoming, CancellationToken ct)
    {
        incoming ??= new();

        await ctx.Set<ImovelCaracteristica>()
            .Where(x => x.ImovelId == imovelId)
            .ExecuteDeleteAsync(ct);

        if (incoming.Count == 0) return;

        var novas = incoming
            .GroupBy(x => x.CaracteristicaId)
            .Select(g => g.First())
            .Select(dto => new ImovelCaracteristica
            {
                Id = Guid.NewGuid(),
                ImovelId = imovelId,
                CaracteristicaId = dto.CaracteristicaId,
                ValorBool = dto.ValorBool,
                ValorInt = dto.ValorInt,
                ValorDecimal = dto.ValorDecimal,
                ValorTexto = dto.ValorTexto,
                ValorData = dto.ValorData,
                Observacao = dto.Observacao
            })
            .ToList();

        await ctx.Set<ImovelCaracteristica>().AddRangeAsync(novas, ct);
    }
}
