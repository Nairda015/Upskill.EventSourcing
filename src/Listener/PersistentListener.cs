using Contracts.Constants;
using Contracts.Settings;
using EventStore.Client;

namespace Listener;

internal class PersistentListener : IHostedService
{
    private readonly EventStorePersistentSubscriptionsClient _client;
    private readonly SnsPublisher _sns;
    private readonly ILogger<PersistentListener> _logger;

    public PersistentListener(
        EventStorePersistentSubscriptionsClient client,
        SnsPublisher sns,
        ILogger<PersistentListener> logger)
    {
        _client = client;
        _sns = sns;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.SubscribeToAllAsync(
            Constants.SubscriptionGroup,
            EventDispatcher,
            SubscriptionDropped,
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Stopping persistent listener");
        return Task.CompletedTask;
    }

    private async Task EventDispatcher(
        PersistentSubscription subscription,
        ResolvedEvent @event,
        int? retryCount,
        CancellationToken cancellationToken)
    {
        if (@event.OriginalEvent.EventType.StartsWith('$'))
        {
            await subscription.Ack(@event);
            return;
        }

        try
        {
            await _sns.PublishAsync(@event, cancellationToken);
            await subscription.Ack(@event);
        }
        catch (Exception ex)
        {
            var strategy = retryCount > 3
                ? PersistentSubscriptionNakEventAction.Park
                : PersistentSubscriptionNakEventAction.Retry;

            await subscription.Nack(strategy, ex.Message, @event);
            _logger.LogError(ex, "Error while handling event {event} with id {id}",
                @event.OriginalEvent.EventType, @event.OriginalEvent.EventId);
        }
    }

    private void SubscriptionDropped(
        PersistentSubscription subscription,
        SubscriptionDroppedReason dropReason,
        Exception? ex)
    {
        _logger.LogWarning("Subscription was dropped due to {dropReason}. {exception}", dropReason, ex?.Message ?? "");
        //TODO: Handle dropped subscription
    }
}