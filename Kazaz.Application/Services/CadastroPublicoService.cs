using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class CadastroPublicoService(ApplicationDbContext ctx) : ICadastroPublicoService
{
    public async Task<ConvitePublicInfoResponse> ObterConviteAsync(string token, CancellationToken ct)
    {
        token = (token ?? "").Trim();
        if (string.IsNullOrWhiteSpace(token))
            return new(false, "Token inválido.", null, null, null, null, null, null);

        var convite = await ctx.Set<ConviteCadastroContrato>()
            .AsNoTracking()
            .Include(x => x.Contrato)
            .FirstOrDefaultAsync(x => x.Token == token, ct);

        if (convite is null)
            return new(false, "Token não encontrado.", null, null, null, null, null, null);

        if (convite.Status != StatusConviteCadastro.Pendente)
            return new(false, $"Convite não está pendente ({convite.Status}).", null, null, null, null, convite.ExpiraEm, null);

        if (convite.ExpiraEm is not null && convite.ExpiraEm < DateTime.UtcNow)
            return new(false, "Convite expirado.", null, null, null, null, convite.ExpiraEm, null);

        if (convite.Contrato.Status != StatusContrato.Rascunho)
            return new(false, "Contrato não está em rascunho.", null, null, null, null, convite.ExpiraEm, null);

        return new(
            true,
            null,
            convite.ContratoId,
            convite.Contrato.Numero,
            convite.Contrato.Tipo,
            convite.Papel,
            convite.ExpiraEm,
            convite.Contrato.ImovelId
        );
    }

    public async Task<FinalizarCadastroPublicoResponse> IniciarAsync(
        string token,
        FinalizarCadastroPublicoRequest request,
        CancellationToken ct)
    {
        token = (token ?? "").Trim();
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Token inválido.");

        if (request is null || string.IsNullOrWhiteSpace(request.Nome))
            throw new InvalidOperationException("Nome é obrigatório.");

        await using var tx = await ctx.Database.BeginTransactionAsync(ct);

        var convite = await ctx.Set<ConviteCadastroContrato>()
            .Include(x => x.Contrato)
                .ThenInclude(c => c.Partes)
            .FirstOrDefaultAsync(x => x.Token == token, ct)
            ?? throw new KeyNotFoundException("Convite não encontrado.");

        if (convite.Status != StatusConviteCadastro.Pendente)
            throw new InvalidOperationException("Convite não está pendente.");

        if (convite.ExpiraEm is not null && convite.ExpiraEm < DateTime.UtcNow)
            throw new InvalidOperationException("Convite expirado.");

        if (convite.Contrato.Status != StatusContrato.Rascunho)
            throw new InvalidOperationException("Contrato não está em rascunho.");

        // ✅ Se já iniciou antes, apenas atualiza dados básicos
        Pessoa pessoa;
        if (convite.PessoaId is Guid pessoaId)
        {
            pessoa = await ctx.Set<Pessoa>().FirstAsync(p => p.Id == pessoaId, ct);
            pessoa.Nome = request.Nome.Trim();
            pessoa.EnderecoId = request.EnderecoId;
        }
        else
        {
            pessoa = new Pessoa
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome.Trim(),
                EnderecoId = request.EnderecoId
            };
            ctx.Set<Pessoa>().Add(pessoa);

            // ✅ Upsert ContratoParte por Papel do convite
            var contrato = convite.Contrato;
            var parte = contrato.Partes.FirstOrDefault(p => p.Papel == convite.Papel);

            if (parte is null)
            {
                contrato.Partes.Add(new ContratoParte
                {
                    Id = Guid.NewGuid(),
                    ContratoId = convite.ContratoId,
                    PessoaId = pessoa.Id,
                    Papel = convite.Papel
                });
            }
            else
            {
                parte.PessoaId = pessoa.Id;
            }

            // ✅ Amarra a pessoa no convite (cadastro iniciado)
            convite.PessoaId = pessoa.Id;
            // ⚠️ Não marca como usado ainda. Isso será no ConcluirAsync.
        }

        // ✅ IMPORTANTE: neste fluxo, ignoramos request.Documentos aqui.
        // Os docs serão anexados depois via /api/documentos usando pessoa.Id.

        await ctx.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new FinalizarCadastroPublicoResponse(convite.ContratoId, pessoa.Id, convite.Papel);
    }

    public async Task<FinalizarCadastroPublicoResponse> ConcluirAsync(string token, CancellationToken ct)
    {
        token = (token ?? "").Trim();
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Token inválido.");

        await using var tx = await ctx.Database.BeginTransactionAsync(ct);

        var convite = await ctx.Set<ConviteCadastroContrato>()
            .Include(x => x.Contrato)
            .FirstOrDefaultAsync(x => x.Token == token, ct)
            ?? throw new KeyNotFoundException("Convite não encontrado.");

        if (convite.Status != StatusConviteCadastro.Pendente)
            throw new InvalidOperationException("Convite não está pendente.");

        if (convite.ExpiraEm is not null && convite.ExpiraEm < DateTime.UtcNow)
            throw new InvalidOperationException("Convite expirado.");

        if (convite.Contrato.Status != StatusContrato.Rascunho)
            throw new InvalidOperationException("Contrato não está em rascunho.");

        if (convite.PessoaId is null)
            throw new InvalidOperationException("Cadastro ainda não foi iniciado.");

        // (Opcional agora; depois melhoramos): validar se tem docs obrigatórios antes de concluir.
        // Ex.: consultar TipoDocumento Obrigatorio e checar PessoaDocumento.

        convite.Status = StatusConviteCadastro.Usado;
        convite.UsadoEm = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new FinalizarCadastroPublicoResponse(
            convite.ContratoId,
            convite.PessoaId.Value,
            convite.Papel
        );
    }
}
