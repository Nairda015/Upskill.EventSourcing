using Commands;
using Commands.Common;
using Commands.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddSingleton<InMemoryDb>();
builder.Services.RegisterHandlers<IApiMarker>();

var app = builder.Build();

app.RegisterEndpoints<IApiMarker>();

app.Run();