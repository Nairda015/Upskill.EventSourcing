using Commands;
using Shared.MiWrap;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager($"/{builder.Environment}/Commands", TimeSpan.FromMinutes(5));
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.RegisterHandlers<IApiMarker>();

var eventStoreSettings = builder.Configuration.GetOptions<EventStoreOptions>();
builder.Services.AddEventStoreClient(eventStoreSettings.ConnectionString,
    x => { x.DefaultDeadline = TimeSpan.FromSeconds(5); });

var app = builder.Build();

app.RegisterEndpoints<IApiMarker>();

app.Run();