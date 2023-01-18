using System;
using System.Threading;
using System.Threading.Tasks;
using Commands.Common;
using Commands.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Commands.Features.Products;

internal record DeleteProduct(Guid Id) : IHttpCommand;

public class DeleteProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapDelete<DeleteProduct, DeleteProductHandler>("{id}")
            .Produces(200)
            .Produces(404);
}

internal class DeleteProductHandler : IHttpCommandHandler<DeleteProduct>
{
    private readonly InMemoryDb _db;

    public DeleteProductHandler(InMemoryDb db) => _db = db;

    public Task<IResult> HandleAsync(DeleteProduct query, CancellationToken cancellationToken)
    {
        if (_db.Products.TryGetValue(query.Id, out var product)) Task.FromResult(Results.Ok(product));

        return Task.FromResult(Results.NotFound());
    }
}