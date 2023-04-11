using System.Text.Json;
using Commands.Features.Categories;
using Contracts.Events.Products;
using EventStore.Client;
using MiWrap;


namespace Commands.Features.Products;

internal record ChangeCategory(ChangeCategory.ChangeCategoryBody Body) : IHttpCommand
{
    internal record ChangeCategoryBody(Guid Id, Guid CategoryId);
}

public class ChangeCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapPut<ChangeCategory, ChangeCategoryHandler>("products/category")
            .Produces(201)
            .Produces(400);
}

internal class ChangeCategoryHandler : IHttpCommandHandler<ChangeCategory>
{
    private readonly EventStoreClient  _client;
    private readonly CategoriesContext _context;
    
    public ChangeCategoryHandler(EventStoreClient client, CategoriesContext context)
    {
        _client = client;
        _context = context;
    }

    public async Task<IResult> HandleAsync(ChangeCategory command, CancellationToken cancellationToken = default)
    {
        var (id, categoryId) = command.Body;
        
        var category = await _context.Categories.FindAsync(new object?[] { categoryId }, cancellationToken);
        if (category is null) return Results.BadRequest();

        var stream = _client.ReadStreamAsync(
            Direction.Forwards,
            id.ToString(),
            StreamPosition.Start,
            cancellationToken: cancellationToken);
        
        if (await stream.ReadState is ReadState.StreamNotFound) return Results.NotFound();
        
        var @event = new CategoryChanged(categoryId, category.Name);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(CategoryChanged),
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            id.ToString(),
            StreamState.StreamExists,
            new[] { eventData },
            cancellationToken: cancellationToken);

        return Results.Accepted();
    }
}