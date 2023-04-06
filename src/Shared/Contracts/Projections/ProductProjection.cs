using Contracts.Events.Products;

namespace Contracts.Projections;

public class ProductProjection
{
    /// <summary>
    /// StreamId and ProductId are the same
    /// </summary>
    public Guid Id { get; init; }

    public string Name { get; init; }
    public Guid CategoryId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public bool IsObsolete { get; set; }
    public bool IsOnSale { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();

    public static ProductProjection Create(ProductCreated productCreated) => new()
    {
        Id = productCreated.Id,
        Name = productCreated.Name,
        CategoryId = productCreated.CategoryId,
        Price = productCreated.Price,
        Description = productCreated.Description,
        IsObsolete = false,
        IsOnSale = false,
        Metadata = new Dictionary<string, string>()
    };

    public ProductProjection Apply(CategoryChanged categoryChanged)
    {
        CategoryId = categoryChanged.CategoryId;
        return this;
    }

    public ProductProjection Apply(DescriptionChanged descriptionChanged)
    {
        Description = descriptionChanged.Description;
        return this;
    }

    public ProductProjection Apply(MarkedAsObsolete _)
    {
        IsObsolete = true;
        return this;
    }

    public ProductProjection Apply(MetadataAdded metadataAdded)
    {
        foreach (var (key, value) in metadataAdded.Value)
        {
            if (Metadata.ContainsKey(key))
            {
                Metadata[key] = value;
            }
            else
            {
                Metadata.Add(key, value);
            }
        }

        return this;
    }

    public ProductProjection Apply(MetadataRemoved metadataRemoved)
    {
        foreach (var (key, _) in metadataRemoved.Value) Metadata.Remove(key);
        return this;
    }

    public ProductProjection Apply(PriceDecreased priceDecreased)
    {
        Price = priceDecreased.NewPrice;
        IsOnSale = priceDecreased.IsPromo;
        return this;
    }

    public ProductProjection Apply(PriceIncreased priceIncreased)
    {
        Price = priceIncreased.NewPrice;
        IsOnSale = priceIncreased.IsPromo;
        return this;
    }
}