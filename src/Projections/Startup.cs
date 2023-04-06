using Amazon;
using Amazon.Lambda.Annotations;
using Contracts.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearch.Net.Auth.AwsSigV4;

namespace Projections;

[LambdaStartup]
public class Startup
{
    private readonly IConfiguration _configuration;
    public Startup() => _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EventsDictionary>();
        
        var openSearchSettings = _configuration
            .GetSection("OpenSearchSettings")
            .Get<OpenSearchSettings>();
        var connection = new AwsSigV4HttpConnection(RegionEndpoint.EUCentral1, service: AwsSigV4HttpConnection.OpenSearchServerlessService);
        var connectionSettings = new ConnectionSettings(openSearchSettings!.Uri, connection)
            .DefaultIndex(Constants.ProductsIndexName)
            .EnableHttpCompression()
            .PrettyJson()
            .DefaultFieldNameInferrer(x => x.ToLower());
        
        services.AddSingleton<IOpenSearchClient>(new OpenSearchClient(connectionSettings));
        services.AddSingleton<IOpenSearchLowLevelClient>(new OpenSearchLowLevelClient(connectionSettings));
        
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<Startup>();
        });
    }
}

public class OpenSearchSettings
{
    public required Uri Uri { get; set; }
}