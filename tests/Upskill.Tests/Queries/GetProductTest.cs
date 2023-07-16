using Contracts.Constants;
using Contracts.Projections;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenSearch.Client;
using OpenSearch.Net;
using Queries.Features.Products;

namespace Upskill.Tests.Queries;

public class GetProductTest
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
        //Arrange
        var productProjection = CreateProductTestHelper.CreateProjectionModel(StreamId, CategoryId);
        await _writClient.IndexAsync<StringResponse>(
            Constants.ProductsIndexName,
            StreamId.ToString(),
            PostData.Serializable(productProjection),
            new IndexRequestParameters());
        var getProductHandler = new GetProductHandler(_readClient);
        
        //Act
        var response = await getProductHandler.HandleAsync(new GetProduct(StreamId), CancellationToken.None);
        var product = response as Ok<ProductProjection>;
        
        //Assert
        product!.Value.Should().BeEquivalentTo(productProjection);

        //Cleanup
        var deleteRequest = new DeleteRequest(Constants.ProductsIndexName, StreamId);
        await _readClient.DeleteAsync(deleteRequest);
    }
}