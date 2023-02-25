using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MiWrap;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapGet<TQuery, THandler>(this IEndpointRouteBuilder endpoints, string template) 
        where TQuery : IHttpQuery
        where THandler : IHttpQueryHandler<TQuery> =>
        endpoints.MapGet(template, async (
                    THandler handler,
                    [AsParameters] TQuery query,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(query, cancellationToken))
            .WithName(typeof(TQuery).FullName!);
    
    public static RouteHandlerBuilder MapPost<TCommand, THandler>(this IEndpointRouteBuilder endpoints, string template) 
        where TCommand : IHttpCommand
        where THandler : IHttpCommandHandler<TCommand> =>
        endpoints.MapPost(template, async (
                    THandler handler,
                    [AsParameters] TCommand command,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(command, cancellationToken))
            .WithName(typeof(TCommand).FullName!);
    
    public static RouteHandlerBuilder MapPut<TCommand, THandler>(this IEndpointRouteBuilder endpoints, string template) 
        where TCommand : IHttpCommand
        where THandler : IHttpCommandHandler<TCommand> =>
        endpoints.MapPut(template, async (
                    THandler handler,
                    [AsParameters] TCommand command,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(command, cancellationToken))
            .WithName(typeof(TCommand).FullName!);
    
    public static RouteHandlerBuilder MapDelete<TCommand, THandler>(this IEndpointRouteBuilder endpoints, string template) 
        where TCommand : IHttpCommand
        where THandler : IHttpCommandHandler<TCommand> =>
        endpoints.MapDelete(template, async (
                    THandler handler,
                    [AsParameters] TCommand command,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(command, cancellationToken))
            .WithName(typeof(TCommand).FullName!);
}