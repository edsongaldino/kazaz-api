using Kazaz.Application;
using Kazaz.Domain.Entities;
using Kazaz.Application.Interfaces.Storage;
using Kazaz.Application.Services;
using Kazaz.Domain.Interfaces;
using Kazaz.Infrastructure;
using Kazaz.Infrastructure.Data;
using Kazaz.Infrastructure.Repositories;
using Kazaz.Infrastructure.Storage;
using Kazaz.SharedKernel;
using Kazaz.SharedKernel.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


// Configurações
var configuration = builder.Configuration;
var jwtSettings = configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

// Serviços
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Kazaz.Domain.Interfaces.ITenantProvider, Kazaz.API.Security.TenantProvider>();

builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
       .UseSnakeCaseNamingConvention()
       .EnableDetailedErrors()
       .EnableSensitiveDataLogging());

builder.Services.AddSharedKernel();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var allowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
        policy =>
        {
            policy.WithOrigins(
                    "https://kazaz.imb.br", 
                    "https://www.kazaz.imb.br",
                    "https://localhost:4200",
                    "http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            // Se tiver cookies/autenticação, usar também .AllowCredentials()
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Kazaz API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // cria/atualiza o schema

    // SEED PERFIS
    var idAdminSistema = new Guid("96030c6a-685b-439f-a0bb-dcdb4e05bf54");
    var idAdminImobiliaria = new Guid("236c9687-ef15-46f3-a21a-e555f1717eb4");
    var idColaborador = new Guid("b47c6a9b-3c3b-4c2c-876b-c72eb1fbb3d8");

    var perfisParaAdicionar = new List<Perfil>();
    if (!db.Perfis.Any(p => p.Id == idAdminSistema))
        perfisParaAdicionar.Add(new Perfil { Id = idAdminSistema, Nome = "Administrador do Sistema" });
    if (!db.Perfis.Any(p => p.Id == idAdminImobiliaria))
        perfisParaAdicionar.Add(new Perfil { Id = idAdminImobiliaria, Nome = "Administrador da Imobiliária" });
    if (!db.Perfis.Any(p => p.Id == idColaborador))
        perfisParaAdicionar.Add(new Perfil { Id = idColaborador, Nome = "Colaborador" });

    if (perfisParaAdicionar.Any())
    {
        db.Perfis.AddRange(perfisParaAdicionar);
        db.SaveChanges();
    }

    // MIGRATE AND REMOVE OLD "Administrador" PROFILE (ID b22cbc7e-c7ae-4f7b-acbc-67f09cc35a3d)
    var idOldAdmin = new Guid("b22cbc7e-c7ae-4f7b-acbc-67f09cc35a3d");
    var oldAdminPerfil = db.Perfis.FirstOrDefault(p => p.Id == idOldAdmin);
    if (oldAdminPerfil != null)
    {
        var usersToMigrate = db.Usuarios.IgnoreQueryFilters().Where(u => u.PerfilId == idOldAdmin).ToList();
        foreach (var u in usersToMigrate)
        {
            u.PerfilId = idAdminImobiliaria;
        }
        db.SaveChanges();

        db.Perfis.Remove(oldAdminPerfil);
        db.SaveChanges();
    }

    // SEED DEFAULT IMOBILIARIA IF EMPTY (TO PRESERVE OLD DATA VINCULATION)
    if (!db.Imobiliarias.Any())
    {
        var defaultImobiliaria = new Imobiliaria
        {
            Id = Guid.NewGuid(),
            RazaoSocial = "Minha Imobiliária S/A",
            NomeFantasia = "Minha Imobiliária",
            Cnpj = "00000000000000",
            Creci = "CRECI 12345",
            DataFundacao = DateTime.UtcNow
        };
        db.Imobiliarias.Add(defaultImobiliaria);
        db.SaveChanges();
    }

    var defaultImob = db.Imobiliarias.FirstOrDefault();

    // SEED ADMIN DO SISTEMA
    var adminEmail = "admin@kazaz.com.br";
    if (!db.Usuarios.Any(u => u.Email == adminEmail))
    {
        var adminUser = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Admin Kazaz",
            Email = adminEmail,
            Senha = PasswordHasher.Hash("Admin@123"),
            Ativo = true,
            PerfilId = idAdminSistema, // Admin do Sistema
            ImobiliariaId = null
        };
        db.Usuarios.Add(adminUser);
        db.SaveChanges();
    }

    // ASSOCIATE EXISTING NULL RECORDS WITH THE DEFAULT IMOBILIARIA (TENANT MIGRATION)
    if (defaultImob != null)
    {
        var defaultImobId = defaultImob.Id.ToString();
        db.Database.ExecuteSqlRaw($"UPDATE usuarios SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL AND perfil_id != '96030c6a-685b-439f-a0bb-dcdb4e05bf54'");
        db.Database.ExecuteSqlRaw($"UPDATE leads SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
        db.Database.ExecuteSqlRaw($"UPDATE pessoas SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
        db.Database.ExecuteSqlRaw($"UPDATE imoveis SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
        db.Database.ExecuteSqlRaw($"UPDATE contratos SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
        db.Database.ExecuteSqlRaw($"UPDATE colaboradores SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
        db.Database.ExecuteSqlRaw($"UPDATE financeiro_lancamentos SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
        db.Database.ExecuteSqlRaw($"UPDATE prestadores_servicos SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
        db.Database.ExecuteSqlRaw($"UPDATE convites_cadastro_contrato SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
        db.Database.ExecuteSqlRaw($"UPDATE checklist_etapas_globais SET imobiliaria_id = '{defaultImobId}' WHERE imobiliaria_id IS NULL");
    }

    // SEED LEADS
    if (!db.Leads.Any())
    {
        var origemCanalPro = db.Origens.FirstOrDefault(o => o.Nome == "Canal Pro");
        if (origemCanalPro == null)
        {
            origemCanalPro = new Origem { Id = Guid.NewGuid(), Nome = "Canal Pro", Descricao = "E-mails recebidos do Canal Pro" };
            db.Origens.Add(origemCanalPro);
        }

        var origemWhatsapp = db.Origens.FirstOrDefault(o => o.Nome == "WhatsApp");
        if (origemWhatsapp == null)
        {
            origemWhatsapp = new Origem { Id = Guid.NewGuid(), Nome = "WhatsApp", Descricao = "Contatos diretos no WhatsApp" };
            db.Origens.Add(origemWhatsapp);
        }

        var imovel = db.Imoveis.FirstOrDefault();

        db.Leads.AddRange(
            new Lead
            {
                Id = Guid.NewGuid(),
                Nome = "Ana Silva",
                Email = "ana.silva@email.com",
                Telefone = "11999999999",
                Status = LeadStatus.Novo,
                Origem = origemCanalPro,
                Imovel = imovel,
                Mensagem = "Olá, estou interessada no apartamento anunciado no Canal Pro.",
                DataCriacao = DateTime.UtcNow.AddHours(-2)
            },
            new Lead
            {
                Id = Guid.NewGuid(),
                Nome = "Bruno Costa",
                Email = "bruno.costa@email.com",
                Telefone = "21988888888",
                Status = LeadStatus.EmAtendimento,
                Origem = origemWhatsapp,
                Mensagem = "Dúvidas sobre as condições de locação direta.",
                DataCriacao = DateTime.UtcNow.AddDays(-1)
            },
            new Lead
            {
                Id = Guid.NewGuid(),
                Nome = "Carlos Oliveira",
                Email = "carlos.oliver@email.com",
                Telefone = "31977777777",
                Status = LeadStatus.Descartado,
                Origem = origemCanalPro,
                Mensagem = "Quero agendar visita mas depois não respondeu.",
                DataCriacao = DateTime.UtcNow.AddDays(-3)
            }
        );

        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kazaz API v1");
    c.RoutePrefix = "swagger"; // acessa em /swagger
});


app.UseHttpsRedirection();

var storagePath = builder.Configuration["Storage:LocalPath"]
    ?? throw new InvalidOperationException("Storage:LocalPath não configurado.");

if (!Directory.Exists(storagePath))
{
    Directory.CreateDirectory(storagePath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storagePath),
    RequestPath = "/api/files"
});

app.UseCors(allowedOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();