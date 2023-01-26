namespace Commands.Events;

public record ProductCreated(
    Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description);