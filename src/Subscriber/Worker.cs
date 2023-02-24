using EventStore.Client;

namespace Subscriber;

public class Worker : BackgroundService
{
    private readonly EventStorePersistentSubscriptionsClient _client;
    private readonly SnsPublisher _sns;
    private readonly ILogger<Worker> _logger;

    public Worker(
        EventStorePersistentSubscriptionsClient client,
        SnsPublisher sns,
        ILogger<Worker> logger)
    {
        _client = client;
        _sns = sns;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _client.SubscribeToAllAsync(
            "af-group",
            EventDispatcher,
            SubscriptionDropped,
            cancellationToken: stoppingToken);
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