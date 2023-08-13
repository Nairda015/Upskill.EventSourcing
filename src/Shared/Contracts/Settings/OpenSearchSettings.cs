namespace Contracts.Settings;

public class OpenSearchSettings : ISettings
{
    public static string SectionName => "OpenSearch";
    public Uri Endpoint { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Username { get; set; } = default!;
}