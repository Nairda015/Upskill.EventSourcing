using Contracts.Constants;
using Contracts.Projections;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenSearch.Client;
using OpenSearch.Net;
using Queries.Features.Products;
using static Upskill.Tests.ConnectionHelper;


namespace Upskill.Tests.Queries;

public class SearchProductByPriceRangeHandlerTest : IAsyncLifetime
{
    private readonly IOpenSearchLowLevelClient _writClient = GetWritClient();
    private readonly IOpenSearchClient _readClient = GetReadClient();
    private static readonly Guid StreamId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();
    
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
        var getProductHandler = new SearchProductByPriceRangeHandler(_readClient);
        await Task.Delay(1000);
        
        //Act
        var response = await getProductHandler.HandleAsync(new SearchProductByPriceRange(100_000, 200_000), CancellationToken.None);
        var product = response as Ok<IReadOnlyCollection<ProductProjection>>;
        
        //Assert
        product!.Value!.FirstOrDefault(x => x.Id == StreamId).Should().BeEquivalentTo(productProjection);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        //Cleanup
        var deleteRequest = new DeleteRequest(Constants.ProductsIndexName, StreamId);
        await _readClient.DeleteAsync(deleteRequest);
    }
}