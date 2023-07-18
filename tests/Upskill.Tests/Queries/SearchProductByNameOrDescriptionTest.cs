using Contracts.Constants;
using Contracts.Projections;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenSearch.Client;
using OpenSearch.Net;
using Queries.Features.Products;

namespace Upskill.Tests.Queries;

public class SearchProductByNameOrDescriptionTest : IAsyncLifetime
{
    private static readonly ConnectionSettings ConnectionSettings = new ConnectionSettings()
        .DefaultIndex(Constants.ProductsIndexName)
        .PrettyJson()
        .DefaultFieldNameInferrer(x => x.ToLower());

    private readonly IOpenSearchLowLevelClient _writClient = new OpenSearchLowLevelClient(ConnectionSettings);
    private readonly IOpenSearchClient _readClient = new OpenSearchClient(ConnectionSettings);
    private static readonly Guid StreamId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();
    
    [Fact]
    public async Task Should_Return_Product()
    {
        //Arrange
        var productProjection = CreateProductTestHelper
            .CreateProjectionModel(StreamId, CategoryId);
        
        await _writClient.IndexAsync<StringResponse>(
            Constants.ProductsIndexName,
            StreamId.ToString(),
            PostData.Serializable(productProjection),
            new IndexRequestParameters());
        var handler = new SearchProductByNameOrDescriptionHandler(_readClient);
        await Task.Delay(1000);
        
        //Act
        var response = await handler.HandleAsync(
            new SearchProductByNameOrDescription("Porsche"),
            CancellationToken.None);
        var product = response as Ok<IReadOnlyCollection<ProductProjection>>;
        
        //Assert
        product!.Value!.FirstOrDefault().Should().BeEquivalentTo(productProjection);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        //Cleanup
        var deleteRequest = new DeleteRequest(Constants.ProductsIndexName, StreamId);
        await _readClient.DeleteAsync(deleteRequest);
    }
}