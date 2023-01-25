namespace Commands.Entities;

public record Product(Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description,
    Dictionary<string, string> Metadata); // 86 87 89 88-> cache 


