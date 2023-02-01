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

    public async Task PublishAsync(ResolvedEvent evnt, CancellationToken cancellationToken)
    {
        var request = new PublishRequest
        {
            TopicArn = _settings.CurrentValue.TopicArn,
            Message = JsonSerializer.Serialize(evnt.OriginalEvent)
        };
        
        request.MessageAttributes.Add("EventType",
            new MessageAttributeValue
            {
                DataType = "String",
                StringValue = evnt.OriginalEvent.EventType
            });

        // foreach (var attribute in message.ToMessageAttributeDictionary())
        // {
        //     request.MessageAttributes.Add(attribute.Key, attribute.Value);
        // }

        await _sns.PublishAsync(request, cancellationToken);
    }
}