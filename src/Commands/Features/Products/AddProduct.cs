using System.Text.Json;
using Commands.Features.Categories;
using Contracts.Events.Products;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using MiWrap;

namespace Commands.Features.Products;

internal record AddProduct(AddProduct.AddProductBody Body) : IHttpCommand
{
    internal record AddProductBody(string Name, decimal Price, Guid CategoryId, string Description);
}

public class AddProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapPost<AddProduct, AddProductHandler>("products")
            .Produces(200)
            .Produces(400);
}

internal class AddProductHandler : IHttpCommandHandler<AddProduct>
{
    private readonly EventStoreClient  _client;
    private readonly CategoriesContext _context;
    
    public AddProductHandler(EventStoreClient client, CategoriesContext context)
    {
        _client = client;
        _context = context;
    }

    public async Task<IResult> HandleAsync(AddProduct command, CancellationToken cancellationToken = default)
    {
        var (name, price, categoryId, description) = command.Body;
        
        var category = await _context.Categories.FindAsync(new object?[] { categoryId }, cancellationToken);
        if (category is null) return Results.BadRequest();
        
        var @event = new ProductCreated(
            Guid.NewGuid(), 
            name, 
            categoryId,
            category.Name,
            price,
            description);

        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(ProductCreated),
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            @event.Id.ToString(),
            StreamState.NoStream,
            new[] { eventData },
            cancellationToken: cancellationToken);

        return Results.Ok(@event.Id);
    }
}