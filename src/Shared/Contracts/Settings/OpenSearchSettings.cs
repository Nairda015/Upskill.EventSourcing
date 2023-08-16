namespace Contracts.Settings;

public class OpenSearchSettings : ISettings
{
    public static string SectionName => "OpenSearch";
    public string Endpoint { get; set; } = default!;
    public Uri Uri => Endpoint.StartsWith("http") ? new Uri(Endpoint) : new Uri($"https://{Endpoint}");
    public string Password { get; set; } = default!;
    public string Username { get; set; } = default!;
}