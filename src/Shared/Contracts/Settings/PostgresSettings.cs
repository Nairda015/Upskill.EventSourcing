namespace Contracts.Settings;

public class PostgresSettings : ISettings
{
    public static string SectionName => "Postgres";
    public string ConnectionString { get; set; } = default!;
    public bool EnableSensitiveData { get; set; }
}