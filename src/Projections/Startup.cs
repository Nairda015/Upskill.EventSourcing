using Amazon;
using Amazon.Lambda.Annotations;
using Contracts.Constants;
using Contracts.Events;
using Contracts.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearch.Net.Auth.AwsSigV4;

namespace Projections;

[LambdaStartup]
public class Startup
{
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .AddSystemsManager("/Upskill/Databases/",TimeSpan.FromMinutes(5))
        .Build();

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EventsDictionary>();

        var openSearchSettings = _configuration.GetOptions<OpenSearchSettings>();
        var connection = new AwsSigV4HttpConnection(RegionEndpoint.EUCentral1, service: AwsSigV4HttpConnection.OpenSearchServerlessService);
        var connectionSettings = new ConnectionSettings(openSearchSettings.Endpoint, connection)
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