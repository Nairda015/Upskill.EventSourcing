namespace Contracts.Settings;

public class OpenSearchSettings : ISettings
{
    public static string SectionName => "OpenSearch";
    public Uri Endpoint { get; set; } = default!;
    
}