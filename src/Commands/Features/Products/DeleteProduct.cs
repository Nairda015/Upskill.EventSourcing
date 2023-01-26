using System.Text.Json;
using Commands.Events;
using EventStore.Client;
using Shared.MiWrap;

namespace Commands.Features.Products;

internal record DeleteProduct(Guid Id) : IHttpCommand;

public class DeleteProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapDelete<DeleteProduct, DeleteProductHandler>("products/{id}")
            .Produces(200)
            .Produces(404);
}

internal class DeleteProductHandler : IHttpCommandHandler<DeleteProduct>
{
    private readonly EventStoreClient _client;

    public DeleteProductHandler(EventStoreClient client)
    {
        _client = client;
    }

    public async Task<IResult> HandleAsync(DeleteProduct command, CancellationToken cancellationToken)
    {
        var stream = _client.ReadStreamAsync(
            Direction.Forwards,
            command.Id.ToString(),
            StreamPosition.Start,
            cancellationToken: cancellationToken);

        if (await stream.ReadState is ReadState.StreamNotFound) return Results.NotFound();
        
        var @event = new MarkAsObsolete(Guid.NewGuid());

        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(MarkAsObsolete),
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            @event.Id.ToString(),
            StreamState.StreamExists,
            new[] { eventData },
            cancellationToken: cancellationToken);

        return Results.Ok(@event.Id);
    }
}