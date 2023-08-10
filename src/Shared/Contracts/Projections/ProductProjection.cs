using Contracts.Events.Products;

namespace Contracts.Projections;

public class ProductProjection
{
    /// <summary>
    /// StreamId and ProductId are the same
    /// All should be private but serialization...
    /// </summary>
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string CategoryName { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;
    public bool IsObsolete { get; set; }
    public bool IsOnSale { get; set; }
    public int Version { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();

    public static ProductProjection Create(ProductCreated productCreated) => new()
    {
        Id = productCreated.Id,
        Name = productCreated.Name,
        CategoryId = productCreated.CategoryId,
        CategoryName = productCreated.CategoryName,
        Price = productCreated.Price,
        Description = productCreated.Description,
        IsObsolete = false,
        IsOnSale = false,
        Version = 0
    };

    public ProductProjection Apply(CategoryChanged categoryChanged)
    {
        CategoryId = categoryChanged.CategoryId;
        CategoryName = categoryChanged.CategoryName;
        Version++;
        return this;
    }

    public ProductProjection Apply(DescriptionChanged descriptionChanged)
    {
        Description = descriptionChanged.Description;
        Version++;
        return this;
    }

    public ProductProjection Apply(MarkedAsObsolete _)
    {
        IsObsolete = true;
        Version++;
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

        Version++;
        return this;
    }

    public ProductProjection Apply(MetadataRemoved metadataRemoved)
    {
        foreach (var (key, _) in metadataRemoved.Value) Metadata.Remove(key);
        Version++;
        return this;
    }

    public ProductProjection Apply(PriceDecreased priceDecreased)
    {
        Price = priceDecreased.NewPrice;
        IsOnSale = priceDecreased.IsPromo;
        Version++;
        return this;
    }

    public ProductProjection Apply(PriceIncreased priceIncreased)
    {
        Price = priceIncreased.NewPrice;
        IsOnSale = priceIncreased.IsPromo;
        Version++;
        return this;
    }
}