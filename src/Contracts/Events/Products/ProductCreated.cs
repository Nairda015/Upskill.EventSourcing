using Contracts.Messages;

namespace Contracts.Events.Products;

// stream name == product id
public record ProductCreated(
    Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description) : ISnsMessage;