using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace Kazaz.SharedKernel;

public static class SharedKernelDependencyInjection
{
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        var assembly = typeof(SharedKernelDependencyInjection).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            // registra todas as classes que terminam com "Service"
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        return services;
    }
}
