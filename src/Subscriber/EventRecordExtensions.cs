using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Contracts.Messages;
using EventStore.Client;

namespace Subscriber;

public static class EventRecordExtensions
{
    public static string ToSnsMessage(this EventRecord @event)
    {
        var streamId = @event.EventStreamId;
        var eventId = @event.EventId.ToGuid();
        var dataAsJson = Encoding.UTF8.GetString(@event.Data.Span);
        var created = @event.Created;
        var number = @event.EventNumber.ToUInt64();

        var metadata = new EventMetadata(streamId, eventId, created, number);
        
        var message = new ExpandoObject();
        message.TryAdd(nameof(SnsMessage<int>.Data), JsonNode.Parse(dataAsJson));
        message.TryAdd(nameof(SnsMessage<int>.Metadata), metadata);

        return JsonSerializer.Serialize(message);
    }
}