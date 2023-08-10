namespace Projections.Cache;

public class DatabaseSettings
{
    public const string KeyName = "DynamoDb";
    public string TableName { get; init; } = default!;
}