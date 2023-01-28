namespace Shared.Settings;

public class EventStoreOptions : IOptions
{
    public static string SectionName => "EventStore";
    public string ConnectionString { get; set; } = default!;
}