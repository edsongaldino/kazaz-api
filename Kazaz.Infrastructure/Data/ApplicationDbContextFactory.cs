using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Kazaz.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // 1) Primeiro tenta variáveis de ambiente (funciona bem em CI/CD)
        //    Ex.: CONNECTIONSTRINGS__DEFAULT="Host=...;Port=...;..."
        var envConn =
            Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULT") ??
            Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION");

        // 2) Define BasePath para conseguir ler appsettings.json (de preferência o da API)
        var cwd = Directory.GetCurrentDirectory();
        var apiPath = Path.Combine(cwd, "..", "Kazaz.API");
        var basePath = Directory.Exists(apiPath) ? apiPath : cwd;

        var cfg = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // 3) Aceita tanto "Default" quanto "DefaultConnection"
        var cs =
            envConn ??
            cfg.GetConnectionString("DefaultConnection") ??
            "Host=localhost;Port=5432;Database=KazazDb;Username=postgres;Password=259864"; // fallback dev

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(cs)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new ApplicationDbContext(options);
    }
}
