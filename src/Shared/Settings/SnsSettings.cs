namespace Shared.Settings;

public class SnsSettings : ISettings
{
    public static string SectionName => "Sns";
    public string TopicArn { get; set; } = default!;
    
    public string Region { get; set; } = default!;
}