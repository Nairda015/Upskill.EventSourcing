namespace Contracts.Events.Products;

public record CategoryChanged(Guid Id, string Name) : IEvent;