using Amazon.Lambda.Annotations;
using Contracts.Events;
using Contracts.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace SqsLambda;

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