using Amazon.Lambda.Annotations;
using Contracts.Events;
using Microsoft.Extensions.DependencyInjection;

namespace LambdaCategories;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<SqsMessagesDictionary>();
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Startup>();
        });
    }
}