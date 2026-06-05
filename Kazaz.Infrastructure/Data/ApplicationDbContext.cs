using Kazaz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace Kazaz.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Perfil> Perfis { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<Cidade> Cidades { get; set; }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<DadosPessoaFisica> DadosPessoasFisicas => Set<DadosPessoaFisica>();
    public DbSet<DadosPessoaJuridica> DadosPessoasJuridicas => Set<DadosPessoaJuridica>();
	public DbSet<DadosComplementares> DadosComplementares => Set<DadosComplementares>();
	public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Imovel> Imoveis => Set<Imovel>();
    public DbSet<Foto> Fotos => Set<Foto>();

    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<PessoaDocumento> PessoasDocumentos => Set<PessoaDocumento>();
    public DbSet<ImovelDocumento> ImoveisDocumentos => Set<ImovelDocumento>();

    public DbSet<Origem> Origens => Set<Origem>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Imobiliaria> Imobiliarias => Set<Imobiliaria>();
    public DbSet<Colaborador> Colaboradores => Set<Colaborador>();
    public DbSet<ColaboradorDocumento> ColaboradoresDocumentos => Set<ColaboradorDocumento>();
    public DbSet<FinanceiroLancamento> FinanceiroLancamentos => Set<FinanceiroLancamento>();
    public DbSet<PrestadorServico> PrestadoresServicos => Set<PrestadorServico>();

    public DbSet<Contato> Contatos => Set<Contato>();
	public DbSet<Conjuge> Conjuge => Set<Conjuge>();

    public DbSet<Caracteristica> Caracteristica => Set<Caracteristica>();

    public DbSet<TipoImovel> TipoImovel => Set<TipoImovel>();

    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<ContratoParte> ContratoPartes => Set<ContratoParte>();
    public DbSet<ContratoChecklistEntrada> ContratoChecklistEntrada => Set<ContratoChecklistEntrada>();
    public DbSet<ContratoChecklistSaida> ContratoChecklistSaida => Set<ContratoChecklistSaida>();
    public DbSet<ChecklistEtapaGlobal> ChecklistEtapasGlobais => Set<ChecklistEtapaGlobal>();

    public DbSet<ImovelProprietario> ImovelProprietarios => Set<ImovelProprietario>();

    public DbSet<RegraDocumentoCadastro> RegraDocumentoCadastro => Set<RegraDocumentoCadastro>();

    private readonly Kazaz.Domain.Interfaces.ITenantProvider? _tenantProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        Kazaz.Domain.Interfaces.ITenantProvider? tenantProvider = null) : base(options)
    {
        _tenantProvider = tenantProvider;
    }


    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filters for multi-tenancy
        mb.Entity<Lead>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<Pessoa>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<Imovel>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<Contrato>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<Colaborador>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<FinanceiroLancamento>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<PrestadorServico>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<Usuario>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<ConviteCadastroContrato>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());
        mb.Entity<ChecklistEtapaGlobal>().HasQueryFilter(x => _tenantProvider == null || _tenantProvider.EhAdminSistema() || x.ImobiliariaId == _tenantProvider.ObterImobiliariaId());

        base.OnModelCreating(mb);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_tenantProvider != null)
        {
            var tenantId = _tenantProvider.ObterImobiliariaId();
            var isSuperAdmin = _tenantProvider.EhAdminSistema();

            foreach (var entry in ChangeTracker.Entries<Kazaz.Domain.Interfaces.IMultiTenant>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (!isSuperAdmin)
                    {
                        entry.Entity.ImobiliariaId = tenantId;
                    }
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        if (_tenantProvider != null)
        {
            var tenantId = _tenantProvider.ObterImobiliariaId();
            var isSuperAdmin = _tenantProvider.EhAdminSistema();

            foreach (var entry in ChangeTracker.Entries<Kazaz.Domain.Interfaces.IMultiTenant>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (!isSuperAdmin)
                    {
                        entry.Entity.ImobiliariaId = tenantId;
                    }
                }
            }
        }

        return base.SaveChanges();
    }
}