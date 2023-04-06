using Contracts.Exceptions;
using Contracts.Messages;

namespace Projections.Cache;

internal class MissingEventException : UpskillException
{
    public MissingEventException(EventMetadata metadata)
        : base($"Event from stream {metadata.StreamId} with number {metadata.Number} is missing")
    {
    }
}