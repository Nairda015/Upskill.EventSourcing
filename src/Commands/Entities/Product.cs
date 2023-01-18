using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.OpenSearchServerless;
using Amazon.OpenSearchServerless.Model;

namespace SimpleApp.Entities;

// MODELS

//is category immutable? -> add and delete only
public record Category(Guid Id, string Name, Guid? ParentId); //where to store this? Dynamo only for me

public record Product(Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description,
    Dictionary<string, string> Metadata); // 86 87 89 88-> cache 

public class Test
{
    public async Task Get(IAmazonOpenSearchServerless client, string nextToken)
    {
        var filter = new CollectionFilters
        {
            Name = "test"

        };
        var req = new ListCollectionsRequest
        {
            CollectionFilters = filter,
            MaxResults = 20,
            NextToken = nextToken
        };
        var res = await client.ListCollectionsAsync(req);
    }
}

// EVENTS
public record CategoryCreated(Guid Id, string Name, Guid? ParentId); // for metric only - no need for storing in Elastic
// flow: Lambda -> Dynamo -> CloudWatch

public record ProductCreated(Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description,
    Dictionary<string, string> Metadata); //metadata is wired in this context - something like tags?

public record MarkAsObsolete(Guid Id);

// READ MODELS
public record ProductDetails(Guid Id,
    string Name,
    string CategoryName,
    decimal Price,
    string Description,
    int Version,
    Dictionary<string, string> Metadata); // what type of information do we need to store - maybe add keys for searching to table? 

public record CategoryReadModel(Guid Id, string Name, Guid? ParentId);

/*
Products Commands:
[] CreateProduct
[] ChangeCategory
[] ChangePrice (split this into multiple with additional metadata for eg promo price)
[] ChangeDescription
[] UpdateMetadata - add, remove
[] MarkAsObsolete

Categories Queries: (for me it looks good for sql db) - Aurora
[] ByName
[] AllChildren
[] ById
[] MainCategories - all without parent

GetProducts Queries: - OpenSearch
[] GetPagedByCategoryWithOrder
[] GetPriceHistory - lowest price from 30 last days :D and charts
[] FullTextSearch - name and description
[] FilterByPrice
[] FilterByMetadata

[] moving data to archives - EFS Infrequent Access
[] logs - CloudWatch
[] alarms for errors with mail notification - CloudWatch

*/