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
    private readonly ILogger<AddCategoryHandler> _logger;

    public AddCategoryHandler(CategoriesContext context, ILogger<AddCategoryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(AddCategory command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Request started for new category with name: {Name}", command.Body.Name);
        var product = new Category(Guid.NewGuid(), command.Body.Name, command.Body.ParentId);

        if (!await IsParentValid(command, product, cancellationToken)) 
            return Results.BadRequest();

        _logger.LogInformation("Parent validated for new category: {Name}", command.Body.Name);

        if (await _context.Categories.AnyAsync(x => x.Name == product.Name && x.ParentId == product.ParentId, cancellationToken))
            return Results.Conflict();

        _logger.LogInformation("Category name validated for new category: {Name}", command.Body.Name);

        await _context.Categories.AddAsync(product, cancellationToken);
        var result = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category added with name: {Name}", command.Body.Name);

        return result is 1
            ? Results.Accepted(product.Id.ToString())
            : Results.BadRequest();
    }

    private async Task<bool> IsParentValid(AddCategory command, Category product, CancellationToken cancellationToken) 
        => command.Body.ParentId is null ||
           await _context.Categories.AnyAsync(x => x.ParentId == product.ParentId, cancellationToken);
}