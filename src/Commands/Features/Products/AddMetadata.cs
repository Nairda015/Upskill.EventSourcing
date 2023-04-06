using System.Text.Json;
using Contracts.Events.Products;
using EventStore.Client;
using MiWrap;

namespace Commands.Features.Products;

internal record AddMetadata(AddMetadata.AddMetadataBody Body) : IHttpCommand
{
    internal record AddMetadataBody(Guid Id, Dictionary<string, string> Metadata);
}

public class AddMetadataEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapPut<AddMetadata, AddMetadataHandler>("products/metadata-add")
            .Produces(201)
            .Produces(400);
}

internal class AddMetadataHandler : IHttpCommandHandler<AddMetadata>
{
    private readonly EventStoreClient  _client;
    
    public AddMetadataHandler(EventStoreClient client)
    {
        _client = client;
    }

    public async Task<IResult> HandleAsync(AddMetadata command, CancellationToken cancellationToken = default)
    {
        var (id, metadata) = command.Body;

        var stream = _client.ReadStreamAsync(
            Direction.Forwards,
            id.ToString(),
            StreamPosition.Start,
            cancellationToken: cancellationToken);
        
        if (await stream.ReadState is ReadState.StreamNotFound) return Results.NotFound();
        
        // var metadata = await stream
        //     .Where(x => x.Event.EventType == nameof(MetadataChanged))
        //     .Select(x => JsonSerializer.Deserialize<MetadataChanged>(x.Event.Data.Span))
        //     .AggregateAsync((c, n) => c.Apply(n.Metadata), cancellationToken);
        
        var @event = new MetadataAdded(metadata);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(MetadataAdded),
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            id.ToString(),
            StreamState.StreamExists,
            new[] { eventData },
            cancellationToken: cancellationToken);

        return Results.Accepted();
    }
}