using Contracts.Constants;
using Contracts.Events.Products;
using Contracts.Projections;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenSearch.Client;
using OpenSearch.Net;
using Queries.Features.Products;

namespace Upskill.Tests.Queries;

public class GetProductByMetadataTest
{
    private static readonly ConnectionSettings ConnectionSettings = new ConnectionSettings()
        .DefaultIndex(Constants.ProductsIndexName)
        .PrettyJson()
        .DefaultFieldNameInferrer(x => x.ToLower());

    private readonly IOpenSearchLowLevelClient _writClient = new OpenSearchLowLevelClient(ConnectionSettings);
    private readonly IOpenSearchClient _readClient = new OpenSearchClient(ConnectionSettings);
    private static readonly Guid StreamId = new("d3f2a3a0-0b7a-4b1a-8b1a-0b9a0b7a4b10");
    private static readonly Guid CategoryId = new("d3f2a3a0-0b7a-4b1a-8b1a-0b9a0b7a4b11");
    
    [Fact]
    public async Task Should_Return_Product()
    {
        const string key = "test";
        const string value = "test-value";
        
        //Arrange
        var productProjection = CreateProductTestHelper
            .CreateProjectionModel(StreamId, CategoryId)
            .Apply(new MetadataAdded(new Dictionary<string, string>{{key, value}}));
        
        await _writClient.IndexAsync<StringResponse>(
            Constants.ProductsIndexName,
            StreamId.ToString(),
            PostData.Serializable(productProjection),
            new IndexRequestParameters());
        var handler = new SearchProductByMetadataHandler(_readClient);
        
        //Act
        var response = await handler.HandleAsync(
            new SearchProductByMetadata($"{key}:{value}"),
            CancellationToken.None);
        var product = response as Ok<IReadOnlyCollection<ProductProjection>>;
        
        //Assert
        product!.Value!.First().Should().BeEquivalentTo(productProjection);
        
        //Cleanup
        var deleteRequest = new DeleteRequest(Constants.ProductsIndexName, StreamId);
        await _readClient.DeleteAsync(deleteRequest);
    }
}