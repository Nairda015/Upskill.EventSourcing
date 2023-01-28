namespace Shared.Settings;

public class PostgresOptions : IOptions
{
    public static string SectionName => "Postgres";
    public string ConnectionString { get; set; } = default!;
}