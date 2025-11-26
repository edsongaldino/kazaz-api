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
    public DbSet<VinculoPessoaImovel> Vinculos => Set<VinculoPessoaImovel>();
    public DbSet<Foto> Fotos => Set<Foto>();
    public DbSet<PerfilVinculoImovel> PerfisVinculoImovel => Set<PerfilVinculoImovel>();

    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<PessoaDocumento> PessoasDocumentos => Set<PessoaDocumento>();
    public DbSet<ImovelDocumento> ImoveisDocumentos => Set<ImovelDocumento>();

    public DbSet<Origem> Origens => Set<Origem>();

    public DbSet<Contato> Contatos => Set<Contato>();
	public DbSet<Conjuge> Conjuge => Set<Conjuge>();


	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(mb);
    }
}