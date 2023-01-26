namespace Commands.Events.Products;

// stream name == product id
public record ProductCreated(
    Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description);