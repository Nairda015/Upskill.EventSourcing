namespace Commands.Entities;

public record Product(Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description,
    Dictionary<string, string> Metadata);


