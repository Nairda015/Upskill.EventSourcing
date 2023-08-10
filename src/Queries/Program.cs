using Amazon;
using Contracts.Constants;
using Contracts.Settings;
using MiWrap;
using OpenSearch.Client;
using OpenSearch.Net.Auth.AwsSigV4;
using Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddSystemsManager(
        $"/{builder.Environment}/Queries",
        TimeSpan.FromMinutes(5));
}

builder.Services.RegisterHandlers<IApiMarker>();
builder.RegisterOptions<PostgresSettings>();

var openSearchSettings = builder.Configuration.GetOptions<OpenSearchSettings>();
var connection = new AwsSigV4HttpConnection(RegionEndpoint.EUCentral1, service: AwsSigV4HttpConnection.OpenSearchServerlessService);
var connectionSettings = new ConnectionSettings(openSearchSettings.Endpoint, connection)
    .DefaultIndex(Constants.ProductsIndexName)
    .EnableHttpCompression()
    .PrettyJson()
    .DefaultFieldNameInferrer(x => x.ToLower());
builder.Services.AddSingleton<IOpenSearchClient>(new OpenSearchClient(connectionSettings));
// var connection = new AwsSigV4HttpConnection(new AssumeRoleAWSCredentials(
//         new BasicAWSCredentials(
//             "aaaaaaaaaaaaaaaaaaaa",
//             "bbbb"
//         ),
//         "cccc",
//         "test"
//     ),
//     RegionEndpoint.EUCentral1);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.OrderActionsBy(x => x.HttpMethod); });

var app = builder.Build();

app.RegisterEndpoints<IApiMarker>();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Upskill"); });

app.Run();