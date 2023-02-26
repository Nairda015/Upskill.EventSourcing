using Commands;
using Commands.Persistence;
using Microsoft.EntityFrameworkCore;
using MiWrap;
using Settings;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddSystemsManager($"/{builder.Environment}/Commands", TimeSpan.FromMinutes(5));
//builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.RegisterHandlers<IApiMarker>();

var eventStoreSettings = builder.Configuration.GetOptions<EventStoreSettings>();
builder.Services.AddEventStorePersistentSubscriptionsClient(eventStoreSettings.ConnectionString);
builder.Services.AddEventStoreClient(eventStoreSettings.ConnectionString,
    x => { x.DefaultDeadline = TimeSpan.FromSeconds(5); });

var options = builder.Configuration.GetOptions<PostgresSettings>();
builder.Services.AddDbContext<CategoriesContext>(ctx =>
    ctx.UseNpgsql(options.ConnectionString)
        .EnableSensitiveDataLogging(options.EnableSensitiveData));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.OrderActionsBy(x => x.HttpMethod); });

var app = builder.Build();
app.MapGet("/", () => "Command healthy");

app.RegisterEndpoints<IApiMarker>();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Upskill"); });

app.Run();