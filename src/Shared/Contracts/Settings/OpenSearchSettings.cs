namespace Contracts.Settings;

public class OpenSearchSettings : ISettings
{
    public static string SectionName => "OpenSearch";
    public string Endpoint { get; set; } = default!;
    public Uri Uri => new(Endpoint);
    public string Password { get; set; } = default!;
    public string Username { get; set; } = default!;
}