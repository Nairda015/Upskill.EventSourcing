namespace Commands.Events.Products;

public record ProductCreated(
    Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description);