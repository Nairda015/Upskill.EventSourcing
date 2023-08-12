using Amazon.SimpleNotificationService;
using Contracts.Settings;
using Listener;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddSystemsManager("/Upskill/Databases/",TimeSpan.FromMinutes(5));
}

builder.Services.AddHostedService<PersistentListener>();

var settings = builder.Configuration.GetOptions<EventStoreSettings>();
builder.Services.AddEventStorePersistentSubscriptionsClient(settings.ConnectionString);

builder.Services.AddSingleton<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();
builder.RegisterOptions<SnsSettings>();
builder.Services.AddSingleton<SnsPublisher>();

var app = builder.Build();

app.MapGet("/", () => "Listener healthy");

app.Run();