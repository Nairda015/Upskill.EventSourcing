using Contracts.Events.Products;
using Contracts.Messages;
using Contracts.Projections;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;
using OpenSearch.Net;
using Projections.Handlers;
using static Upskill.Tests.ConnectionHelper;

namespace Upskill.Tests.Projections.Handlers;

public class ProductCreatedHandlerTests
{
    private readonly IOpenSearchLowLevelClient _writClient = GetWritClient();
    private readonly IOpenSearchClient _readClient = GetReadClient();

    [Fact]
    public async Task CreateProductTest()
    {
        //Arrange
        
        //Common
        var productId = new Guid("d3f2a3a0-0b7a-4b1a-8b1a-0b9a0b7a4b00");
        var categoryId = new Guid("d3f2a3a0-0b7a-4b1a-8b1a-0b9a0b7a4b01");
        var metadata = new EventMetadata(productId.ToString(), Guid.NewGuid(), DateTime.UtcNow, 1);
        
        //Request data
        var productCreated = new ProductCreated(productId, 
            "Porsche",
            categoryId, 
            "Car", 
            1000, 
            "Super car");
        
        var newCategoryId = new Guid("d3f2a3a0-0b7a-4b1a-8b1a-0b9a0b7a4b02");
        var categoryChanged = new CategoryChanged(newCategoryId, "super-car");
        var descriptionChanged = new DescriptionChanged("German super car");
        var markedAsObsolete = new MarkedAsObsolete();
        var metadataAdded = new MetadataAdded(new Dictionary<string, string>
        {
            {"color", "red"}, {"max-speed", "300"}
        });
        var metadataRemoved = new MetadataRemoved(new Dictionary<string, string> {{"color", ""}});
        var priceIncreased = new PriceIncreased(200_000, false);
        var priceDecreased = new PriceDecreased(150_000, true);
        
        //Requests
        var productCreatedRequest = new SnsMessage<ProductCreated>(productCreated, metadata);
        var categoryChangedRequest = new SnsMessage<CategoryChanged>(categoryChanged, metadata);
        var descriptionChangedRequest = new SnsMessage<DescriptionChanged>(descriptionChanged, metadata);
        var markedAsObsoleteRequest = new SnsMessage<MarkedAsObsolete>(markedAsObsolete, metadata);
        var metadataAddedRequest = new SnsMessage<MetadataAdded>(metadataAdded, metadata);
        var metadataRemovedRequest = new SnsMessage<MetadataRemoved>(metadataRemoved, metadata);
        var priceIncreasedRequest = new SnsMessage<PriceIncreased>(priceIncreased, metadata);
        var priceDecreasedRequest = new SnsMessage<PriceDecreased>(priceDecreased, metadata);
        
        //Handlers
        var productCreatedHandler = new ProductCreatedHandler(Substitute.For<ILogger<ProductCreatedHandler>>(), _writClient);
        var categoryChangedHandler = new CategoryChangedHandler(Substitute.For<ILogger<CategoryChangedHandler>>(), _readClient);
        var descriptionChangedHandler = new DescriptionChangedHandler(Substitute.For<ILogger<DescriptionChangedHandler>>(), _readClient);
        var markedAsObsoleteHandler = new MarkedAsObsoleteHandler(Substitute.For<ILogger<MarkedAsObsoleteHandler>>(), _readClient);
        var metadataChangedHandler = new MetadataChangedHandler(Substitute.For<ILogger<MetadataChangedHandler>>(), _readClient);
        var priceIncreasedHandler = new PriceIncreasedHandler(Substitute.For<ILogger<PriceIncreasedHandler>>(), _readClient);
        var priceDecreasedHandler = new PriceDecreasedHandler(Substitute.For<ILogger<PriceDecreasedHandler>>(), _readClient);

        //Act
        await productCreatedHandler.Handle(productCreatedRequest, new CancellationToken());
        await categoryChangedHandler.Handle(categoryChangedRequest, new CancellationToken());
        await descriptionChangedHandler.Handle(descriptionChangedRequest, new CancellationToken());
        await markedAsObsoleteHandler.Handle(markedAsObsoleteRequest, new CancellationToken());
        await metadataChangedHandler.Handle(metadataAddedRequest, new CancellationToken());
        await metadataChangedHandler.Handle(metadataRemovedRequest, new CancellationToken());
        await priceIncreasedHandler.Handle(priceIncreasedRequest, new CancellationToken());
        await priceDecreasedHandler.Handle(priceDecreasedRequest, new CancellationToken());
        
        var response = await _readClient.GetAsync<ProductProjection>(productId, x => x);
        
        //Assert
        response.Should().NotBeNull();
        response.Source.Should().BeEquivalentTo(new ProductProjection()
        {
            Id = productId,
            Price = 150_000,
            IsOnSale = true,
            CategoryName = "super-car",
            Name = "Porsche",
            Version = 7,
            CategoryId = newCategoryId,
            Description = "German super car",
            IsObsolete = true,
            Metadata = new Dictionary<string, string> {{"max-speed", "300"}}
            
        });
        //response.Should()
        // var response = await _client.IndexAsync<StringResponse>(Constants.ProductsIndexName, request.Metadata.StreamId,
        //     PostData.Serializable(data), new IndexRequestParameters(), cancellationToken);

    }
}