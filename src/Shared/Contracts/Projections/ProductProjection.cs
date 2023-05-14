using Contracts.Events.Products;

namespace Contracts.Projections;

public class ProductProjection
{
    /// <summary>
    /// StreamId and ProductId are the same
    /// </summary>
    public Guid Id { get; private init; }
    public string Name { get; private init; } = default!;
    public string CategoryName { get; private set; } = default!;
    public Guid CategoryId { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; } = default!;
    public bool IsObsolete { get; private set; }
    public bool IsOnSale { get; private set; }
    public int Version { get; private set; }
    public Dictionary<string, string> Metadata { get; } = new();

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
        CategoryId = categoryChanged.Id;
        CategoryName = categoryChanged.Name;
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