using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Kazaz.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class LeadService(
    ApplicationDbContext db,
    IPessoaService pessoaService,
    IPessoaFisicaService pfService,
    IPessoaJuridicaService pjService,
    IContatoService contatoService
) : ILeadService
{
    public async Task<LeadResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var lead = await db.Leads
            .AsNoTracking()
            .Include(x => x.Origem)
            .Include(x => x.Imovel)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Lead não encontrado.");

        return Map(lead);
    }

    public async Task<LeadsPagedResponse> SearchAsync(LeadSearchFilterDto filter, CancellationToken ct)
    {
        var query = db.Leads
            .AsNoTracking()
            .Include(x => x.Origem)
            .Include(x => x.Imovel)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Nome))
        {
            var nomeUpper = filter.Nome.ToUpperInvariant();
            query = query.Where(x => x.Nome.ToUpper().Contains(nomeUpper));
        }

        if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            var emailUpper = filter.Email.ToUpperInvariant();
            query = query.Where(x => x.Email != null && x.Email.ToUpper().Contains(emailUpper));
        }

        if (!string.IsNullOrWhiteSpace(filter.Telefone))
        {
            query = query.Where(x => x.Telefone != null && x.Telefone.Contains(filter.Telefone));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.OrigemId.HasValue)
        {
            query = query.Where(x => x.OrigemId == filter.OrigemId.Value);
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.DataCriacao)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        var totalNovo = await db.Leads.CountAsync(x => x.Status == LeadStatus.Novo, ct);
        var totalEmAtendimento = await db.Leads.CountAsync(x => x.Status == LeadStatus.EmAtendimento, ct);
        var totalConvertido = await db.Leads.CountAsync(x => x.Status == LeadStatus.Convertido, ct);
        var totalDescartado = await db.Leads.CountAsync(x => x.Status == LeadStatus.Descartado, ct);

        return new LeadsPagedResponse(
            items.Select(Map).ToList(),
            filter.Page,
            filter.PageSize,
            total,
            totalNovo,
            totalEmAtendimento,
            totalConvertido,
            totalDescartado
        );
    }

    public async Task<Guid> CreateAsync(LeadCreateDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(dto.Nome));

        if (dto.OrigemId.HasValue)
        {
            var origemExiste = await db.Origens.AnyAsync(x => x.Id == dto.OrigemId.Value, ct);
            if (!origemExiste) throw new ArgumentException("Origem não encontrada.");
        }

        if (dto.ImovelId.HasValue)
        {
            var imovelExiste = await db.Imoveis.AnyAsync(x => x.Id == dto.ImovelId.Value, ct);
            if (!imovelExiste) throw new ArgumentException("Imóvel não encontrado.");
        }

        var lead = new Lead
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Email = dto.Email?.Trim(),
            Telefone = dto.Telefone?.Trim(),
            OrigemId = dto.OrigemId,
            ImovelId = dto.ImovelId,
            Status = dto.Status ?? LeadStatus.Novo,
            Mensagem = dto.Mensagem?.Trim(),
            DataCriacao = DateTime.UtcNow
        };

        db.Leads.Add(lead);
        await db.SaveChangesAsync(ct);

        return lead.Id;
    }

    public async Task UpdateAsync(Guid id, LeadUpdateDto dto, CancellationToken ct)
    {
        var lead = await db.Leads.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Lead não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(dto.Nome));

        if (dto.OrigemId.HasValue)
        {
            var origemExiste = await db.Origens.AnyAsync(x => x.Id == dto.OrigemId.Value, ct);
            if (!origemExiste) throw new ArgumentException("Origem não encontrada.");
        }

        if (dto.ImovelId.HasValue)
        {
            var imovelExiste = await db.Imoveis.AnyAsync(x => x.Id == dto.ImovelId.Value, ct);
            if (!imovelExiste) throw new ArgumentException("Imóvel não encontrado.");
        }

        lead.Nome = dto.Nome.Trim();
        lead.Email = dto.Email?.Trim();
        lead.Telefone = dto.Telefone?.Trim();
        lead.OrigemId = dto.OrigemId;
        lead.ImovelId = dto.ImovelId;
        lead.Status = dto.Status;
        lead.Mensagem = dto.Mensagem?.Trim();
        lead.DataAtualizacao = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var lead = await db.Leads.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Lead não encontrado.");

        db.Leads.Remove(lead);
        await db.SaveChangesAsync(ct);
    }

    public async Task<Guid> ConvertToClientAsync(Guid leadId, ConvertLeadRequest request, CancellationToken ct)
    {
        var lead = await db.Leads.FirstOrDefaultAsync(x => x.Id == leadId, ct)
            ?? throw new KeyNotFoundException("Lead não encontrado.");

        if (lead.Status == LeadStatus.Convertido || lead.PessoaId.HasValue)
            throw new InvalidOperationException("Este Lead já foi convertido em cliente.");

        using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            // 1. Criar Pessoa base com Origem do Lead
            var pessoaId = await pessoaService.CriarBaseAsync(lead.Nome, null, lead.OrigemId, ct);

            // 2. Criar Dados Pessoa Física ou Jurídica
            var cleanDocumento = request.Documento != null ? string.Concat(request.Documento.Where(char.IsDigit)) : string.Empty;

            if (request.TipoPessoa.ToUpperInvariant() == "PF")
            {
                var pfDto = new DadosPessoaFisicaDto(
                    Cpf: string.IsNullOrWhiteSpace(cleanDocumento) ? string.Empty : cleanDocumento.PadLeft(11, '0'),
                    Rg: string.Empty,
                    Nome: lead.Nome,
                    OrgaoExpedidor: string.Empty,
                    DataNascimento: null,
                    EstadoCivil: EstadoCivil.NaoInformado,
                    Nacionalidade: "Brasileira"
                );
                await pfService.CriarAsync(pessoaId, pfDto, ct);
            }
            else
            {
                var pjDto = new DadosPessoaJuridicaDto(
                    NomeFantasia: lead.Nome,
                    RazaoSocial: lead.Nome,
                    Cnpj: string.IsNullOrWhiteSpace(cleanDocumento) ? string.Empty : cleanDocumento.PadLeft(14, '0'),
                    InscricaoEstadual: string.Empty,
                    DataAbertura: null
                );
                await pjService.CriarAsync(pessoaId, pjDto, ct);
            }

            // 3. Criar Contatos do Lead para o novo cliente (email / telefone)
            var contatos = new List<ContatoDto>();
            if (!string.IsNullOrWhiteSpace(lead.Email))
            {
                contatos.Add(new ContatoDto("EMAIL", lead.Email.Trim(), true));
            }
            if (!string.IsNullOrWhiteSpace(lead.Telefone))
            {
                contatos.Add(new ContatoDto("TELEFONE", lead.Telefone.Trim(), contatos.Count == 0));
            }

            if (contatos.Count > 0)
            {
                await contatoService.CriarVariosAsync(pessoaId, contatos, ct);
            }

            // 4. Atualizar Lead
            lead.PessoaId = pessoaId;
            lead.Status = LeadStatus.Convertido;
            lead.DataAtualizacao = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return pessoaId;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private static LeadResponseDto Map(Lead lead)
    {
        return new LeadResponseDto(
            lead.Id,
            lead.Nome,
            lead.Email,
            lead.Telefone,
            lead.OrigemId,
            lead.Origem?.Nome,
            lead.ImovelId,
            lead.Imovel?.Codigo,
            lead.Status,
            lead.Mensagem,
            lead.PessoaId,
            lead.DataCriacao,
            lead.DataAtualizacao
        );
    }
}
