using System.Text.Json;
using Contracts.Events.Products;
using EventStore.Client;
using MiWrap;

namespace Commands.Features.Products;

internal record ChangeDescription(ChangeDescription.ChangeDescriptionBody Body) : IHttpCommand
{
    internal record ChangeDescriptionBody(Guid Id, string Description);
}

public class ChangeDescriptionEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapPut<ChangeDescription, ChangeDescriptionHandler>("products/description")
            .Produces(200)
            .Produces(400);
}

internal class ChangeDescriptionHandler : IHttpCommandHandler<ChangeDescription>
{
    private readonly EventStoreClient  _client;
    
    public ChangeDescriptionHandler(EventStoreClient client)
    {
        _client = client;
    }

    public async Task<IResult> HandleAsync(ChangeDescription command, CancellationToken cancellationToken = default)
    {
        var (id, description) = command.Body;

        var stream = _client.ReadStreamAsync(
            Direction.Forwards,
            id.ToString(),
            StreamPosition.Start,
            cancellationToken: cancellationToken);
        
        if (await stream.ReadState is ReadState.StreamNotFound) return Results.NotFound();
        
        var @event = new DescriptionChanged(description);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(DescriptionChanged),
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            id.ToString(),
            StreamState.StreamExists,
            new[] { eventData },
            cancellationToken: cancellationToken);

        return Results.Accepted();
    }
}