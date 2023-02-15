using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Builder;
using Shared.Settings;
using Subscriber;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddEnvironmentVariables();

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddSystemsManager($"/{environmentName}/Commands", TimeSpan.FromMinutes(5));
}


builder.Services.AddHostedService<Worker>();

var settings = builder.Configuration.GetOptions<EventStoreSettings>();
builder.Services.AddEventStorePersistentSubscriptionsClient(settings.ConnectionString);

builder.Services.AddAWSService<IAmazonSimpleNotificationService>();
builder.Services.RegisterOptions<SnsSettings>(builder.Configuration);
builder.Services.AddSingleton<SnsPublisher>();

var app = builder.Build();

app.Run();