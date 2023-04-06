namespace Projections.Cache;

public class DatabaseSettings
{
    public const string KeyName = "DynamoDb";
    public required string TableName { get; init; }
}