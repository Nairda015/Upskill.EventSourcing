using Commands.Entities;
using Commands.Persistence;
using Shared.MiWrap;

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

        await _context.Categories.AddAsync(product, cancellationToken);
        var result = await _context.SaveChangesAsync(cancellationToken);

        return result is 1
            ? Results.Accepted()
            : Results.BadRequest(); //TODO: validation or smth - probably parent does not exist
    }
}