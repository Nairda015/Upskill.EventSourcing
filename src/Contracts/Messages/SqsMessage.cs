namespace Contracts.Messages;

public record SnsMessage<T>(T Data, EventMetadata EventMetadata) : ISnsMessage;