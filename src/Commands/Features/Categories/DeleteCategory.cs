using Commands.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.MiWrap;

namespace Commands.Features.Categories;

internal record DeleteCategory(Guid Id) : IHttpCommand;

public class DeleteCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapDelete<DeleteCategory, DeleteCategoryHandler>("category/{id}")
            .Produces(200)
            .Produces(404);
}

internal class DeleteCategoryHandler : IHttpCommandHandler<DeleteCategory>
{
    private readonly CategoriesContext _context;

    public DeleteCategoryHandler(CategoriesContext context)
    {
        _context = context;
    }

    public async Task<IResult> HandleAsync(DeleteCategory query, CancellationToken cancellationToken)
    {
        //if any product has this category, it cannot be deleted - opensearch

        var result = await _context.Categories
            .Where(x => x.Id == query.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return result is 1
            ? Results.Ok()
            : Results.NotFound();
    }
}