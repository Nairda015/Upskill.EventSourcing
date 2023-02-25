namespace Settings;

public class EventStoreSettings : ISettings
{
    public static string SectionName => "EventStore";
    public string ConnectionString { get; set; } = default!;
}