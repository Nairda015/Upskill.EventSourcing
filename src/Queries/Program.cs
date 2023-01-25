using Queries;
using Shared.MiWrap;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.RegisterHandlers<IApiMarker>();

var app = builder.Build();

app.RegisterEndpoints<IApiMarker>();

app.Run();