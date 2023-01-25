namespace Queries.Models;

public record ProductDetails(Guid Id,
    string Name,
    string CategoryName,
    decimal Price,
    string Description,
    int Version,
    Dictionary<string, string> Metadata);