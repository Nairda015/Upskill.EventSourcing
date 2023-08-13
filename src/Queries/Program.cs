using Contracts.Constants;
using Contracts.Settings;
using MiWrap;
using OpenSearch.Client;
using Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddSystemsManager("/Upskill/Databases/",TimeSpan.FromMinutes(5));
}

builder.Services.RegisterHandlers<IApiMarker>();
builder.RegisterOptions<PostgresSettings>();

var openSearchSettings = builder.Configuration.GetOptions<OpenSearchSettings>();
var connectionSettings = new ConnectionSettings(openSearchSettings.Endpoint)
    .DefaultIndex(Constants.ProductsIndexName)
    .EnableHttpCompression()
    .PrettyJson()
    .DefaultFieldNameInferrer(x => x.ToLower());
if (!builder.Environment.IsDevelopment()) connectionSettings.BasicAuthentication(
    openSearchSettings.Username,
    openSearchSettings.Password);
builder.Services.AddSingleton<IOpenSearchClient>(new OpenSearchClient(connectionSettings));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.OrderActionsBy(x => x.HttpMethod); });

var app = builder.Build();

app.RegisterEndpoints<IApiMarker>();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Upskill"); });

app.Run();