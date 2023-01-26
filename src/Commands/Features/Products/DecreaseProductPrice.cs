using System.Text.Json;
using Commands.Events.Products;
using EventStore.Client;
using Shared.MiWrap;

namespace Commands.Features.Products;


internal record DecreaseProductPrice(DecreaseProductPrice.DecreaseProductPriceBody Body) : IHttpCommand
{
    internal record DecreaseProductPriceBody(Guid Id, decimal Price, bool IsPromo);
}

public class DecreaseProductPriceEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
        builder.MapPost<DecreaseProductPrice, DecreaseProductPriceHandler>("products/decrease-price")
            .Produces(201)
            .Produces(404);
}

internal class DecreaseProductPriceHandler : IHttpCommandHandler<DecreaseProductPrice>
{
    private readonly EventStoreClient _client;

    public DecreaseProductPriceHandler(EventStoreClient client)
    {
        _client = client;
    }

    public async Task<IResult> HandleAsync(DecreaseProductPrice command, CancellationToken cancellationToken = default)
    {
        var (id, newPrice, isPromo) = command.Body;

        var stream = _client.ReadStreamAsync(
            Direction.Forwards,
            id.ToString(),
            StreamPosition.Start,
            cancellationToken: cancellationToken);
        
        if (await stream.ReadState is ReadState.StreamNotFound) return Results.NotFound();
        
        var @event = new PriceDecreased(newPrice, isPromo);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(PriceDecreased),
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            id.ToString(),
            StreamState.StreamExists,
            new[] { eventData },
            cancellationToken: cancellationToken);

        return Results.Accepted();
    }
}