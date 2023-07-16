namespace Contracts.Events.Products;

public record CategoryChanged(Guid CategoryId, string CategoryName) : IEvent;