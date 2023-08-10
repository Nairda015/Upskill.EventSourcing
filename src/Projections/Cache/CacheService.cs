using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Contracts.Messages;
using Microsoft.Extensions.Options;

namespace Projections.Cache;

internal sealed class CacheService
{
    private readonly IAmazonDynamoDB _db;
    private readonly IOptions<DatabaseSettings> _databaseSettings;

    public CacheService(IAmazonDynamoDB db, IOptions<DatabaseSettings> databaseSettings)
    {
        _db = db;
        _databaseSettings = databaseSettings;
    }

    public async Task<EventsList> GetCachedEvents(EventMetadata metadata)
    {
        var request = new GetItemRequest
        {
            TableName = _databaseSettings.Value.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = metadata.StreamId } },
                { "sk", new AttributeValue { S = metadata.StreamId } }
            }
        };

        var response = await _db.GetItemAsync(request);
        if (!response.IsItemSet || response.Item.Count == 0) return EventsList.Default(metadata);

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<EventsList>(itemAsDocument.ToJson())!;
    }

    public async Task<bool> UpdateStream(EventsList request)
    {
        var customerAsJson = JsonSerializer.Serialize(request);
        var itemAsDocument = Document.FromJson(customerAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _databaseSettings.Value.TableName,
            Item = itemAsAttributes
        };

        var response = await _db.PutItemAsync(createItemRequest);
        return response.HttpStatusCode is HttpStatusCode.OK;
    }

    public async Task<DynamoEvent> GetEvent(EventMetadata metadata)
    {
        var request = new GetItemRequest
        {
            TableName = _databaseSettings.Value.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = metadata.StreamId } },
                { "sk", new AttributeValue { S = metadata.Number.ToString() } }
            }
        };

        var response = await _db.GetItemAsync(request);
        if (response.Item.Count == 0) throw new MissingEventException(metadata);

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<DynamoEvent>(itemAsDocument.ToJson())!;
    }
    
    public async Task<bool> SaveEvent(DynamoEvent request)
    {
        var customerAsJson = JsonSerializer.Serialize(request);
        var itemAsDocument = Document.FromJson(customerAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _databaseSettings.Value.TableName,
            Item = itemAsAttributes
        };

        var response = await _db.PutItemAsync(createItemRequest);
        return response.HttpStatusCode is HttpStatusCode.OK;
    }
    
    public async Task<bool> DeleteEvent(EventMetadata request)
    {
        var deleteItemRequest = new DeleteItemRequest
        {
            TableName = _databaseSettings.Value.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = request.StreamId } },
                { "sk", new AttributeValue { S = request.Number.ToString() } }
            }
        };

        var response = await _db.DeleteItemAsync(deleteItemRequest);
        return response.HttpStatusCode is HttpStatusCode.OK;
    }
}