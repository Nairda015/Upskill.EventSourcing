using Commands;
using Shared.MiWrap;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.RegisterHandlers<IApiMarker>();

// var client = GetEventStoreConnection(Configuration["eventStore:connectionString"])
// services.AddSingleton(client);

var app = builder.Build();

app.RegisterEndpoints<IApiMarker>();

app.Run();