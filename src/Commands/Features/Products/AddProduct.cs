using System.Text.Json;
using Commands.Events.Products;
using Commands.Persistence;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Shared.MiWrap;

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
        if (!await _context.Categories.AnyAsync(x => x.Id == categoryId, cancellationToken))
            return Results.BadRequest();
        
        var @event = new ProductCreated(
            Guid.NewGuid(), 
            name, 
            categoryId,
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