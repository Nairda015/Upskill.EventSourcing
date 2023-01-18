using System;
using System.Threading;
using System.Threading.Tasks;
using Commands.Common;
using Commands.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
namespace Commands.Features.Products;

internal record GetProduct(Guid Id) : IHttpQuery;

public class GetProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapGet<GetProduct, GetProductHandler>("{id}")
            .Produces(200)
            .Produces(404);
}

internal class GetProductHandler : IHttpQueryHandler<GetProduct>
{
    private readonly InMemoryDb _db;

    public GetProductHandler(InMemoryDb db) => _db = db;

    public Task<IResult> HandleAsync(GetProduct query, CancellationToken cancellationToken)
    {
        if (_db.Products.TryGetValue(query.Id, out var product)) Task.FromResult(Results.Ok(product));

        return Task.FromResult(Results.NotFound());
    }
}