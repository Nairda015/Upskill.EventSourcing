using Contracts.Events.Products;
using Contracts.Projections;

namespace Upskill.Tests.Queries;

public class CreateProductTestHelper
{
    public static ProductProjection CreateProjectionModel(Guid streamId, Guid categoryId)
    {
        //stream id and product id are the same
        var productCreated = new ProductCreated(
            streamId, 
            "Porsche",
            categoryId, 
            "Car", 
            200_000, 
            "Super car");

        var metadataAdded = new MetadataAdded(new Dictionary<string, string>
        {
            {"color", "red"},
            {"max-speed", "300"},
        });
        
        var priceDecreased = new PriceDecreased(150_000, true);
        
        return ProductProjection
            .Create(productCreated)
            .Apply(metadataAdded)
            .Apply(priceDecreased);
    }
}