using System.Text.Json;
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
    
    public ChangeCategoryHandler(EventStoreClient client)
    {
        _client = client;
    }

    public async Task<IResult> HandleAsync(ChangeCategory command, CancellationToken cancellationToken = default)
    {
        var (id, categoryId) = command.Body;

        var stream = _client.ReadStreamAsync(
            Direction.Forwards,
            id.ToString(),
            StreamPosition.Start,
            cancellationToken: cancellationToken);
        
        if (await stream.ReadState is ReadState.StreamNotFound) return Results.NotFound();
        
        var @event = new CategoryChanged(categoryId);
        
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