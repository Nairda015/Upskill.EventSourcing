using Commands.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.MiWrap;

namespace Commands.Features.Categories;

internal record DeleteProduct(Guid Id) : IHttpCommand;

public class DeleteProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapDelete<DeleteProduct, DeleteProductHandler>("category/{id}")
            .Produces(200)
            .Produces(404);
}

internal class DeleteProductHandler : IHttpCommandHandler<DeleteProduct>
{
    private readonly CategoriesContext _context;

    public DeleteProductHandler(CategoriesContext context)
    {
        _context = context;
    }

    public async Task<IResult> HandleAsync(DeleteProduct query, CancellationToken cancellationToken)
    {
        //if any product has this category, it cannot be deleted

        var result = await _context.Categories
            .Where(x => x.Id == query.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        return result is 1
            ? Results.Ok()
            : Results.NotFound();
    }
}