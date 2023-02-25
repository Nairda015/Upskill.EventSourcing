using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Contracts.Constants;
using EventStore.Client;
using Microsoft.Extensions.Options;
using Settings;

namespace Listener;

internal class SnsPublisher
{
    private readonly ILogger<SnsPublisher> _logger;
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly IOptionsMonitor<SnsSettings> _topicSettings;
    private string? _topicArn;

    public SnsPublisher(
        IAmazonSimpleNotificationService sns,
        IOptionsMonitor<SnsSettings> topicSettings,
        ILogger<SnsPublisher> logger)
    {
        _sns = sns;
        _topicSettings = topicSettings;
        _logger = logger;
    }

    public async Task PublishAsync(ResolvedEvent @event, CancellationToken cancellationToken)
    {
        var topicArn = await GetTopicArnAsync();

        var request = new PublishRequest
        {
            TopicArn = topicArn,
            Message = @event.OriginalEvent.ToSnsMessage()
        };

        request.MessageAttributes.Add(AttributesNames.MessageType, new MessageAttributeValue
        {
            DataType = "String",
            StringValue = @event.OriginalEvent.EventType
        });

        try
        {
            await _sns.PublishAsync(request, cancellationToken);
            _logger.LogInformation(
                "Published event {event} with id {id}",
                @event.OriginalEvent.EventType,
                @event.OriginalEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error while publishing event {event} with id {id}",
                @event.OriginalEvent.EventType,
                @event.OriginalEvent.EventId);
            throw;
        }
    }
    
    private async ValueTask<string> GetTopicArnAsync()
    {
        if (_topicArn is not null) return _topicArn;

        var queueUrlResponse = await _sns.FindTopicAsync(_topicSettings.CurrentValue.Name);
        _topicArn = queueUrlResponse.TopicArn;
        return _topicArn;
    }
}