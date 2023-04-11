namespace Contracts.Events.Products;

// stream name == product id
public record ProductCreated(
    Guid Id,
    string Name,
    Guid CategoryId,
    string CategoryName,
    decimal Price,
    string Description) : IEvent;