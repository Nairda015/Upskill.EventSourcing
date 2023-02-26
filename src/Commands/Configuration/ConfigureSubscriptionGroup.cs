using Contracts.Constants;
using EventStore.Client;
using MiWrap;

namespace Commands.Configuration;

internal record ConfigureSubscriptionGroup : IHttpCommand;

public class ConfigureSubscriptionGroupEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder)
        => builder.MapPost<ConfigureSubscriptionGroup, ConfigureSubscriptionGroupHandler>(
            "configure-subscription-group");
}

internal class ConfigureSubscriptionGroupHandler : IHttpCommandHandler<ConfigureSubscriptionGroup>
{
    private readonly EventStorePersistentSubscriptionsClient _client;
    public ConfigureSubscriptionGroupHandler(EventStorePersistentSubscriptionsClient client) => _client = client;

    public async Task<IResult> HandleAsync(ConfigureSubscriptionGroup query, CancellationToken cancellationToken)
    {
        var settings = new PersistentSubscriptionSettings();
        
        await _client.CreateToAllAsync(
            Constants.SubscriptionGroup,
            settings,
            cancellationToken: cancellationToken);
        
        return Results.Ok();
    }
}