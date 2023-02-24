using System.Text.Json;
using Contracts.Events.Products;
using EventStore.Client;
using MiWrap;

namespace Commands.Features.Products;


internal record IncreaseProductPrice(IncreaseProductPrice.IncreaseProductPriceBody Body) : IHttpCommand
{
    internal record IncreaseProductPriceBody(Guid Id, decimal Price, bool IsPromo);
}

public class IncreaseProductPriceEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapPut<IncreaseProductPrice, IncreaseProductPriceHandler>("products/increase-price")
            .Produces(201)
            .Produces(404);
}

internal class IncreaseProductPriceHandler : IHttpCommandHandler<IncreaseProductPrice>
{
    private readonly EventStoreClient _client;

    public IncreaseProductPriceHandler(EventStoreClient client)
    {
        _client = client;
    }

    public async Task<IResult> HandleAsync(IncreaseProductPrice command, CancellationToken cancellationToken = default)
    {
        var (id, newPrice, isPromo) = command.Body;

        var stream = _client.ReadStreamAsync(
            Direction.Forwards,
            id.ToString(),
            StreamPosition.Start,
            cancellationToken: cancellationToken);
        
        if (await stream.ReadState is ReadState.StreamNotFound) return Results.NotFound();
        
        var @event = new PriceIncreased(newPrice, isPromo);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(PriceIncreased),
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            id.ToString(),
            StreamState.StreamExists,
            new[] { eventData },
            cancellationToken: cancellationToken);

        return Results.Accepted();
    }
}