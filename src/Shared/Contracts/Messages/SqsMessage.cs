namespace Contracts.Messages;

public record SnsMessage<T>(T Data, EventMetadata Metadata) : ISnsMessage;

public record EventMetadata(string StreamId, Guid Id, DateTime Created, ulong Number);