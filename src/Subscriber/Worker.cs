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
        while (!stoppingToken.IsCancellationRequested)
        {
            await _client.SubscribeToAllAsync(
                "all",
                EventDispatcher,
                SubscriptionDropped,
                cancellationToken: stoppingToken);

            // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task EventDispatcher(
        PersistentSubscription subscription,
        ResolvedEvent evnt,
        int? retryCount,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sns.PublishAsync(evnt, cancellationToken);
            await subscription.Ack(evnt);
        }
        catch (Exception ex)
        {
            var strategy = retryCount > 3
                ? PersistentSubscriptionNakEventAction.Park
                : PersistentSubscriptionNakEventAction.Retry;

            await subscription.Nack(strategy, ex.Message, evnt);
            _logger.LogError(ex, "Error while handling event {event} with id {id}",
                evnt.OriginalEvent.EventType, evnt.OriginalEvent.EventId);
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

    // private async Task CreateSubscriptionGroup(CancellationToken cancellationToken)
    // {
    //     var settings = new PersistentSubscriptionSettings();
    //
    //     var userCredentials = new UserCredentials("admin", "changeit");
    //     var filter = StreamFilter.Prefix("test");
    //
    //     await _client.CreateToAllAsync(
    //         "subscription-group",
    //         filter,
    //         settings,
    //         userCredentials: userCredentials,
    //         cancellationToken: cancellationToken);
    // }
}