using System.Text.Json;
using Commands.Events.Products;
using EventStore.Client;
using Shared.MiWrap;

namespace Commands.Features.Products;

internal record RemoveMetadata(RemoveMetadata.RemoveMetadataBody Body) : IHttpCommand
{
    internal record RemoveMetadataBody(Guid Id, Dictionary<string, string> Metadata);
}

public class RemoveMetadataEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapDelete<RemoveMetadata, RemoveMetadataHandler>("products/metadata")
            .Produces(201)
            .Produces(400);
}

internal class RemoveMetadataHandler : IHttpCommandHandler<RemoveMetadata>
{
    private readonly EventStoreClient _client;

    public RemoveMetadataHandler(EventStoreClient client)
    {
        _client = client;
    }

    public async Task<IResult> HandleAsync(RemoveMetadata command, CancellationToken cancellationToken = default)
    {
        var (id, metadata) = command.Body;

        var stream = _client.ReadStreamAsync(
            Direction.Forwards,
            id.ToString(),
            StreamPosition.Start,
            cancellationToken: cancellationToken);

        if (await stream.ReadState is ReadState.StreamNotFound) return Results.NotFound();

        var @event = new MetadataAdded(metadata);

        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(MetadataChanged),
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            id.ToString(),
            StreamState.StreamExists,
            new[] { eventData },
            cancellationToken: cancellationToken);

        return Results.Accepted();
    }
}