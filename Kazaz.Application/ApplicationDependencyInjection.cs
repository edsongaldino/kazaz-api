// ApplicationDependencyInjection.cs
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Kazaz.Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var appAssembly = typeof(ApplicationDI).Assembly; // Kazaz.Application
        services.Scan(scan => scan
            .FromAssemblies(appAssembly)
            // Services: classes que terminam com "Service"
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        // Validators/Handlers/etc. – adicione mais convenções se quiser:
        //.AddClasses(c => c.Where(t => t.Name.EndsWith("Validator")))
        //    .AsImplementedInterfaces()
        //    .WithTransientLifetime()
        );

        return services;
    }
}
