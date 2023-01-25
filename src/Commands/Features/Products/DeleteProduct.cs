using Commands.Persistence;
using Shared.MiWrap;

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
    private readonly CategoriesContext _db;

    public DeleteProductHandler(CategoriesContext db) => _db = db;

    public Task<IResult> HandleAsync(DeleteProduct query, CancellationToken cancellationToken)
    {
        //if (_db.CategoriesContext.TryGetValue(query.Id, out var product)) Task.FromResult(Results.Ok(product));

        return Task.FromResult(Results.NotFound());
    }
}