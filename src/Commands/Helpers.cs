using System.Text.Json;
using Commands.Events.Products;
using EventStore.Client;

namespace Commands;

public static class Helpers
{
    // public static object? Deserialize(this ResolvedEvent resolvedEvent)
    // {
    //     //var dataType = TypeMapper.GetType(resolvedEvent.Event.EventType);
    //     //var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
    //     //return JsonSerializer.Deserialize(jsonData, dataType);
    // }
    
    public static async Task<MetadataChanged> AggregateMetadata(EventStoreClient.ReadStreamResult stream,
        CancellationToken cancellationToken)
    {
        return await stream.Where(x => x.Event.EventType == nameof(MetadataChanged))
            .Select(x => JsonSerializer.Deserialize<MetadataChanged>(x.Event.Data.Span))
            .AggregateAsync((c, n) => c.Apply(n.Metadata), cancellationToken);
    }
}