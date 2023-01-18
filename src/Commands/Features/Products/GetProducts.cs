using System.Threading;
using System.Threading.Tasks;
using Commands.Common;
using Commands.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Commands.Features.Products;

public record GetProducts : IHttpQuery;


public class GetProductsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetProducts, GetProductsHandler>("")
            .Produces(200);
}

internal class GetProductsHandler : IHttpQueryHandler<GetProducts>
{
    private readonly InMemoryDb _db;

    public GetProductsHandler(InMemoryDb db) => _db = db;

    public Task<IResult> HandleAsync(GetProducts query, CancellationToken cancellationToken) =>
        Task.FromResult(Results.Ok(_db.Products));
}