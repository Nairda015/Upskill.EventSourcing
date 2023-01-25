using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.MiWrap;

public static class EndpointsExtensions
{
    public static void RegisterHandlers<T>(this IServiceCollection services)
        where T : class
    {
        var assembly = Assembly.GetAssembly(typeof(T))!;
        
        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IHttpQueryHandler<>)))
            .AsSelf()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IHttpCommandHandler<>)))
            .AsSelf()
            .WithScopedLifetime());
    }
    
    public static void RegisterEndpoints<T>(this IEndpointRouteBuilder endpoints)
        where T : class
    {
        var assembly = Assembly.GetAssembly(typeof(T));
        var moduleEndpoints = assembly!
            .GetTypes()
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && x.IsClass)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>()
            .ToList();
        
        moduleEndpoints.ForEach(x => x.RegisterEndpoint(endpoints));
    }
}