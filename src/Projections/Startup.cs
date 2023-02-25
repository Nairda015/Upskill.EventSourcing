using Amazon.Lambda.Annotations;
using Contracts.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Projections;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EventsDictionary>();
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Startup>();
        });
    }
}