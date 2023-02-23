using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using EventStore.Client;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Subscriber;

public class SnsPublisher
{
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly IOptionsMonitor<SnsSettings> _settings;

    public SnsPublisher(IAmazonSimpleNotificationService sns, IOptionsMonitor<SnsSettings> settings)
    {
        _sns = sns;
        _settings = settings;
    }

    public async Task PublishAsync(ResolvedEvent @event, CancellationToken cancellationToken)
    {
        var request = new PublishRequest
        {
            TopicArn = _settings.CurrentValue.TopicArn,
            Message = JsonSerializer.Serialize(@event.OriginalEvent)
        };

        request.MessageAttributes.Add("MessageType", new MessageAttributeValue
        {
            DataType = "String",
            StringValue = @event.OriginalEvent.EventType
        });
        
        await _sns.PublishAsync(request, cancellationToken);
    }
}