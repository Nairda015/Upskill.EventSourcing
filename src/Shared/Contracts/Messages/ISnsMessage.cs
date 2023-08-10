using MediatR;

namespace Contracts.Messages;

public interface ISnsMessage : IRequest
{
    EventMetadata Metadata { get; init; }
}