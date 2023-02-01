using Amazon.SimpleNotificationService;
using Shared.Settings;
using Subscriber;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((ctx, config) =>
{
    config.AddJsonFile("appsettings.json", false, true);
    config.AddEnvironmentVariables();

    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if (environmentName == Environments.Development)
    {
        config.AddSystemsManager($"/{environmentName}/Commands", TimeSpan.FromMinutes(5));
    }
});

builder.ConfigureServices((ctx, services) =>
{
    services.AddHostedService<Worker>();

    var settings = ctx.Configuration.GetOptions<EventStoreSettings>();
    services.AddEventStorePersistentSubscriptionsClient(settings.ConnectionString);
    
    //services.AddDefaultAWSOptions(ctx.Configuration.GetAWSOptions());
    services.AddAWSService<IAmazonSimpleNotificationService>();
    services.RegisterOptions<SnsSettings>(ctx.Configuration);
    services.AddSingleton<SnsPublisher>();
});

var host = builder.Build();

host.Run();