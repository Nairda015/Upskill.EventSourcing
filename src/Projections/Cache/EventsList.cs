using Contracts.Messages;

namespace Projections.Cache;

internal class EventsList
{
    public string Pk => StreamId;
    public string Sk => StreamId;
    public required string StreamId { get; set; }
    public required ulong NextExpectedNumber { get; set; }
    private readonly SortedSet<ulong> _events = new();

    public static EventsList Default(EventMetadata newEvent) => new()
    {
        StreamId = newEvent.StreamId,
        NextExpectedNumber = newEvent.Number
    };

    public bool ShouldStartProcessing() => _events.Contains(NextExpectedNumber);

    public SortedSet<ulong> ExtractReadyToProcess()
    {
        var eventsToProcess = new SortedSet<ulong>();
        while (_events.Contains(NextExpectedNumber))
        {
            eventsToProcess.Add(NextExpectedNumber);
            _events.Remove(NextExpectedNumber);
            NextExpectedNumber++;
        }

        return eventsToProcess;
    }

    public void Add(ISnsMessage request) => _events.Add(request.Metadata.Number);
}