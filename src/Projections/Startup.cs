using Contracts.Constants;
using Contracts.Events;
using Contracts.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;
using OpenSearch.Net;

namespace Projections;

public static class Startup
{
    public static IConfiguration ConfigureAppConfiguration() => new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .AddSystemsManager("/Upskill/Databases/",TimeSpan.FromMinutes(5))
        .Build();
    
    public static IServiceProvider ConfigureServices(IConfiguration config)
    {
        var services = new ServiceCollection();
        services.AddSingleton(config);
        services.AddSingleton(LoggerFactory.Create(builder => builder.AddConsole()));
        services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

        services.AddSingleton<EventsDictionary>();

        var openSearchSettings = config.GetOptions<OpenSearchSettings>();
        var connectionSettings = new ConnectionSettings(openSearchSettings.Uri)
            .DefaultIndex(Constants.ProductsIndexName)
            .BasicAuthentication(openSearchSettings.Username, openSearchSettings.Password)
            .EnableHttpCompression()
            .PrettyJson()
            .DefaultFieldNameInferrer(x => x.ToLower());
        
        services.AddSingleton<IOpenSearchClient>(new OpenSearchClient(connectionSettings));
        services.AddSingleton<IOpenSearchLowLevelClient>(new OpenSearchLowLevelClient(connectionSettings));

        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Function>();
        });
        
        services.AddDefaultAWSOptions(config.GetAWSOptions());
    
        return services.BuildServiceProvider();
    }
}