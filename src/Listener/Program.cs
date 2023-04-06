using Amazon.SimpleNotificationService;
using Listener;
using Settings;

var builder = WebApplication.CreateBuilder(args);

// if (!builder.Environment.IsDevelopment())
// {
//     builder.Configuration.AddSystemsManager($"/{builder.Environment.EnvironmentName}/Commands", TimeSpan.FromMinutes(5));
// }

builder.Services.AddHostedService<PersistentListener>();

var settings = builder.Configuration.GetOptions<EventStoreSettings>();
builder.Services.AddEventStorePersistentSubscriptionsClient(settings.ConnectionString);

builder.Services.AddSingleton<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();
builder.RegisterOptions<SnsSettings>();
builder.Services.AddSingleton<SnsPublisher>();

var app = builder.Build();

app.Run();