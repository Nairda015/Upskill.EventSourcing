using Commands;
using Commands.Features.Categories;
using Contracts.Settings;
using Microsoft.EntityFrameworkCore;
using MiWrap;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddSystemsManager("/Upskill/Databases/",TimeSpan.FromMinutes(5));
}

builder.Services.RegisterHandlers<IApiMarker>();
var eventStoreSettings = builder.Configuration.GetOptions<EventStoreSettings>();
builder.Services.AddEventStorePersistentSubscriptionsClient(eventStoreSettings.ConnectionString);
builder.Services.AddEventStoreClient(eventStoreSettings.ConnectionString,
    x => { x.DefaultDeadline = TimeSpan.FromSeconds(5); });

var postgresSettings = builder.Configuration.GetOptions<PostgresSettings>();
builder.Services.AddDbContext<CategoriesContext>(ctx =>
    ctx.UseNpgsql(postgresSettings.ConnectionString)
        .EnableSensitiveDataLogging(postgresSettings.EnableSensitiveData));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.OrderActionsBy(x => x.HttpMethod); });

var app = builder.Build();
app.MapGet("/", () => "Command healthy");

app.RegisterEndpoints<IApiMarker>();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Upskill"); });

app.Run();