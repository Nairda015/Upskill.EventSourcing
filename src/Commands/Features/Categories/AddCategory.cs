using Commands.Entities;
using Commands.Persistence;
using Microsoft.EntityFrameworkCore;
using MiWrap;

namespace Commands.Features.Categories;

internal record AddCategory(AddCategory.AddCategoryBody Body) : IHttpCommand
{
    internal record AddCategoryBody(string Name, Guid? ParentId);
}

public class AddProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapPost<AddCategory, AddCategoryHandler>("category")
            .Produces(201);
}

internal class AddCategoryHandler : IHttpCommandHandler<AddCategory>
{
    private readonly CategoriesContext _context;

    public AddCategoryHandler(CategoriesContext context)
    {
        _context = context;
    }

    public async Task<IResult> HandleAsync(AddCategory command, CancellationToken cancellationToken = default)
    {
        var product = new Category(Guid.NewGuid(), command.Body.Name, command.Body.ParentId);

        if (!await IsParentValid(command, product, cancellationToken)) 
            return Results.BadRequest();

        if (await _context.Categories.AnyAsync(x => x.Name == product.Name && x.ParentId == product.ParentId, cancellationToken))
            return Results.Conflict();

        await _context.Categories.AddAsync(product, cancellationToken);
        var result = await _context.SaveChangesAsync(cancellationToken);

        return result is 1
            ? Results.Accepted(product.Id.ToString())
            : Results.BadRequest();
    }

    private async Task<bool> IsParentValid(AddCategory command, Category product, CancellationToken cancellationToken) 
        => command.Body.ParentId is null ||
           await _context.Categories.AnyAsync(x => x.ParentId == product.ParentId, cancellationToken);
}