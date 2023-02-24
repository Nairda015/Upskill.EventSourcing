using EventStore.Client;
using Shared.MiWrap;

namespace Commands.Configuration;

internal record ConfigureSubscriptionGroup
    (ConfigureSubscriptionGroup.ConfigureSubscriptionGroupBody Body) : IHttpCommand
{
    internal record ConfigureSubscriptionGroupBody(string GroupName);
}

public class ConfigureSubscriptionGroupEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder builder)
        => builder.MapPost<ConfigureSubscriptionGroup, ConfigureSubscriptionGroupHandler>(
            "configure-subscription-group/{groupName}");
}

internal class ConfigureSubscriptionGroupHandler : IHttpCommandHandler<ConfigureSubscriptionGroup>
{
    private readonly EventStorePersistentSubscriptionsClient _client;
    public ConfigureSubscriptionGroupHandler(EventStorePersistentSubscriptionsClient client) => _client = client;

    public async Task<IResult> HandleAsync(ConfigureSubscriptionGroup query, CancellationToken cancellationToken)
    {
        var settings = new PersistentSubscriptionSettings();
        
        await _client.CreateToAllAsync(
            query.Body.GroupName,
            settings,
            cancellationToken: cancellationToken);
        
        return Results.Ok();
    }
}