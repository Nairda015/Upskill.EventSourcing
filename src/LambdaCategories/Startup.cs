using Amazon.Lambda.Annotations;
using Contracts.Events;
using Contracts.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace LambdaCategories;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<SnsMessagesDictionary>();
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Startup>();
        });
    }
}