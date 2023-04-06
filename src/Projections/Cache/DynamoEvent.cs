using System.Text.Json;
using Contracts.Messages;

namespace Projections.Cache;

internal class DynamoEvent
{
    public string Pk => StreamId;
    public string Sk => Number.ToString();
    public required string StreamId { get; init; }
    public required ulong Number { get; init; }
    public required string TypeName { get; init; }

    /// <summary>
    /// SnsMessage as json
    /// </summary>
    public required string Data { get; init; }
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