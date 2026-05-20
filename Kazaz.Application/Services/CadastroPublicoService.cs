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

        if (convite.Status != StatusConviteCadastro.PendentePreenchimento &&
            convite.Status != StatusConviteCadastro.CorrecaoSolicitada)
            return new(false, $"Convite não está pendente ou sob correção ({convite.Status}).", null, null, null, null, convite.ExpiraEm, null);

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

        if (convite.Status != StatusConviteCadastro.PendentePreenchimento &&
            convite.Status != StatusConviteCadastro.CorrecaoSolicitada)
            throw new InvalidOperationException("Convite não está pendente ou sob correção.");

        if (convite.ExpiraEm is not null && convite.ExpiraEm < DateTime.UtcNow)
            throw new InvalidOperationException("Convite expirado.");

        if (convite.Contrato.Status != StatusContrato.Rascunho)
            throw new InvalidOperationException("Contrato não está em rascunho.");

        // ✅ Se já iniciou antes, apenas atualiza dados básicos
        Pessoa pessoa;
        if (convite.PessoaId is Guid pessoaId)
        {
            pessoa = await ctx.Set<Pessoa>().FirstAsync(p => p.Id == pessoaId, ct);
            pessoa.EnderecoId = request.EnderecoId;
        }
        else
        {
            pessoa = new Pessoa
            {
                Id = Guid.NewGuid(),
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

        if (convite.Status != StatusConviteCadastro.PendentePreenchimento &&
            convite.Status != StatusConviteCadastro.CorrecaoSolicitada)
            throw new InvalidOperationException("Convite não está pendente ou sob correção.");

        if (convite.ExpiraEm is not null && convite.ExpiraEm < DateTime.UtcNow)
            throw new InvalidOperationException("Convite expirado.");

        if (convite.Contrato.Status != StatusContrato.Rascunho)
            throw new InvalidOperationException("Contrato não está em rascunho.");

        if (convite.PessoaId is null)
            throw new InvalidOperationException("Cadastro ainda não foi iniciado.");

        // (Opcional agora; depois melhoramos): validar se tem docs obrigatórios antes de concluir.
        // Ex.: consultar TipoDocumento Obrigatorio e checar PessoaDocumento.

        convite.Status = StatusConviteCadastro.Preenchido;
        convite.UsadoEm = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new FinalizarCadastroPublicoResponse(
            convite.ContratoId,
            convite.PessoaId.Value,
            convite.Papel
        );
    }

    public async Task<CadastroPublicoStatusResponse> ObterStatusAsync(string token, CancellationToken ct)
    {
        var convite = await ctx.Set<ConviteCadastroContrato>()
            .AsNoTracking()
            .Include(x => x.Analises)
            .FirstOrDefaultAsync(x => x.Token == token, ct)
            ?? throw new KeyNotFoundException("Convite não encontrado.");

        var iniciado = convite.PessoaId.HasValue && convite.PessoaId.Value != Guid.Empty;
        var concluido = convite.Status == StatusConviteCadastro.Preenchido;

        var ultimoComentario = convite.Analises
            .OrderByDescending(a => a.CriadoEm)
            .Select(a => a.Comentario)
            .FirstOrDefault();

        return new CadastroPublicoStatusResponse(
            convite.ContratoId,
            convite.PessoaId,
            convite.Papel,
            concluido,
            iniciado,
            convite.Status,
            ultimoComentario
        );
    }

    public async Task VincularPessoaAsync(
        string token,
        Guid pessoaId,
        CancellationToken ct)
    {
        var convite = await ctx.Set<ConviteCadastroContrato>()
            .FirstOrDefaultAsync(x => x.Token == token, ct)
            ?? throw new KeyNotFoundException("Convite não encontrado.");

        // 🔒 regras de negócio
        if (convite.Status == StatusConviteCadastro.Cancelado ||
            convite.Status == StatusConviteCadastro.Expirado)
        {
            throw new InvalidOperationException("Convite não está mais válido.");
        }

        // evita sobrescrever vínculo existente
        if (convite.PessoaId.HasValue && convite.PessoaId != pessoaId)
        {
            throw new InvalidOperationException("Este convite já está vinculado a outra pessoa.");
        }

        convite.PessoaId = pessoaId;
        //convite.Status = StatusConviteCadastro.Usado;
        //convite.UsadoEm = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
    }

    public async Task<CadastroPublicoDetalhesResponse> ObterDetalhesAsync(string token, CancellationToken ct)
    {
        token = (token ?? "").Trim();
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Token inválido.");

        var convite = await ctx.Set<ConviteCadastroContrato>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == token, ct)
            ?? throw new KeyNotFoundException("Convite não encontrado.");

        if (convite.PessoaId is null || convite.PessoaId == Guid.Empty)
            return new CadastroPublicoDetalhesResponse(null, []);

        var pessoa = await ctx.Set<Pessoa>()
            .AsNoTracking()
            .Include(x => x.Endereco)
            .Include(x => x.PessoaFisica)
            .Include(x => x.PessoaJuridica)
            .Include(x => x.DadosComplementares)
            .Include(x => x.Conjuge)
            .Include(x => x.Contatos)
            .FirstOrDefaultAsync(x => x.Id == convite.PessoaId.Value, ct);

        PessoaDetailsDto? pessoaDto = null;

        if (pessoa is not null)
        {
            var isPf = pessoa.PessoaFisica is not null;
            var isPj = pessoa.PessoaJuridica is not null;

            if (isPf || isPj)
            {
                var nome = isPf
                    ? pessoa.PessoaFisica!.Nome
                    : (pessoa.PessoaJuridica!.NomeFantasia ?? pessoa.PessoaJuridica!.RazaoSocial);

                var documento = isPf
                    ? pessoa.PessoaFisica!.Cpf
                    : pessoa.PessoaJuridica!.Cnpj;

                pessoaDto = new PessoaDetailsDto(
                    Id: pessoa.Id,
                    TipoPessoa: isPf ? "PF" : "PJ",
                    Nome: nome,
                    Documento: documento,
                    OrigemId: pessoa.OrigemId,

                    Endereco: pessoa.Endereco is null ? null : new EnderecoResponseDto
                    {
                        Id = pessoa.Endereco.Id,
                        Cep = pessoa.Endereco.Cep,
                        Logradouro = pessoa.Endereco.Logradouro,
                        Numero = pessoa.Endereco.Numero,
                        Complemento = pessoa.Endereco.Complemento,
                        Bairro = pessoa.Endereco.Bairro,
                        CidadeId = pessoa.Endereco.CidadeId
                    },

                    DadosPessoaFisica: pessoa.PessoaFisica is null ? null : new DadosPessoaFisicaDto(
                        Nome: pessoa.PessoaFisica.Nome,
                        Cpf: pessoa.PessoaFisica.Cpf,
                        DataNascimento: pessoa.PessoaFisica.DataNascimento,
                        Rg: pessoa.PessoaFisica.Rg,
                        OrgaoExpedidor: pessoa.PessoaFisica.OrgaoExpedidor,
                        Nacionalidade: pessoa.PessoaFisica.Nacionalidade,
                        EstadoCivil: pessoa.PessoaFisica.EstadoCivil
                    ),

                    DadosPessoaJuridica: pessoa.PessoaJuridica is null ? null : new DadosPessoaJuridicaDto(
                        RazaoSocial: pessoa.PessoaJuridica.RazaoSocial,
                        NomeFantasia: pessoa.PessoaJuridica.NomeFantasia,
                        Cnpj: pessoa.PessoaJuridica.Cnpj,
                        DataAbertura: pessoa.PessoaJuridica.DataAbertura,
                        InscricaoEstadual: pessoa.PessoaJuridica.InscricaoEstadual
                    ),

                    Contatos: (pessoa.Contatos ?? Enumerable.Empty<Contato>())
                        .OrderByDescending(c => c.Principal)
                        .ThenBy(c => c.Tipo)
                        .Select(c => new ContatoDto(
                            Tipo: c.Tipo,
                            Valor: c.Valor,
                            Principal: c.Principal
                        ))
                        .ToList(),

                    DadosComplementares: pessoa.DadosComplementares is null ? null : new DadosComplementaresDto(
                        Profissao: pessoa.DadosComplementares.Profissao,
                        Escolaridade: pessoa.DadosComplementares.Escolaridade,
                        RendaMensal: pessoa.DadosComplementares.RendaMensal,
                        Observacoes: pessoa.DadosComplementares.Observacoes
                    ),

                    Conjuge: pessoa.Conjuge is null ? null : new ConjugeDto(
                        Id: pessoa.Conjuge.Id,
                        Nome: pessoa.Conjuge.Nome,
                        Cpf: pessoa.Conjuge.Cpf,
                        Rg: pessoa.Conjuge.Rg,
                        OrgaoExpedidor: pessoa.Conjuge.OrgaoExpedidor,
                        DataNascimento: pessoa.Conjuge.DataNascimento,
                        Telefone: pessoa.Conjuge.Telefone,
                        Email: pessoa.Conjuge.Email
                    )
                );
            }
        }

        var documentos = await (
            from pd in ctx.Set<PessoaDocumento>().AsNoTracking()
            join d in ctx.Set<Documento>().AsNoTracking() on pd.DocumentoId equals d.Id
            join td in ctx.Set<TipoDocumento>().AsNoTracking() on pd.TipoDocumentoId equals td.Id
            where pd.PessoaId == convite.PessoaId.Value
               && pd.ContratoId == convite.ContratoId
            orderby td.Nome, d.Nome
            select new DocumentoVisualizacaoDto(
                d.Id,
                td.Id,
                td.Nome,
                d.Nome,
                d.Caminho,
                d.ContentType
            )
        ).ToListAsync(ct);

        return new CadastroPublicoDetalhesResponse(pessoaDto, documentos);
    }

}
