using System.Text.Json;
using Contracts.Messages;

namespace Projections.Cache;

internal class DynamoEvent
{
    public string Pk => StreamId;
    public string Sk => Number.ToString();
    public string StreamId { get; init; } = default!;
    public ulong Number { get; init; } = default!;
    public string TypeName { get; init; } = default!;

    /// <summary>
    /// SnsMessage as json
    /// </summary>
    public string Data { get; init; }  = default!;
}

internal static class SnsMessageExtensions
{
    public static DynamoEvent CreateFromMessage(this ISnsMessage message) => new()
    {
        StreamId = message.Metadata.StreamId,
        Number = message.Metadata.Number,
        TypeName = message.GetType().Name,
        Data = JsonSerializer.Serialize(message)
    };
}