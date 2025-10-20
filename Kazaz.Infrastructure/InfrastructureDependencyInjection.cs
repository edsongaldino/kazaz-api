using Microsoft.Extensions.DependencyInjection;
using Scrutor;
namespace Kazaz.Infrastructure;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var infraAssembly = typeof(InfrastructureDI).Assembly;
        services.Scan(scan => scan
            .FromAssemblies(infraAssembly)
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        return services;
    }
}
