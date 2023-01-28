using Queries;
using Shared.MiWrap;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);


//builder.Configuration.AddSystemsManager($"/{builder.Environment}/Commands", TimeSpan.FromMinutes(5));
//builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.RegisterHandlers<IApiMarker>();

builder.RegisterOptions<PostgresOptions>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.OrderActionsBy(x => x.HttpMethod); });

var app = builder.Build();

app.RegisterEndpoints<IApiMarker>();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGroceryStore"); });

app.Run();