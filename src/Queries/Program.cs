using Queries;
using Shared.MiWrap;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddSystemsManager($"/{builder.Environment}/Commands", TimeSpan.FromMinutes(5));
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.RegisterHandlers<IApiMarker>();

var app = builder.Build();

app.RegisterEndpoints<IApiMarker>();

app.Run();