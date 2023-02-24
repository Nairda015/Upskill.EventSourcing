namespace Shared.Settings;

public class SnsSettings : ISettings
{
    public static string SectionName => "Sns";
    public required string Name { get; set; }
    
    public string Region { get; set; } = default!;
}