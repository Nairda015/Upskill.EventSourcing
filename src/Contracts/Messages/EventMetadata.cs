namespace Contracts.Messages;

public record EventMetadata(
    string EventStreamId,
    Guid EventId,
    DateTime Created);