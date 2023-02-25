namespace Contracts.Events.Products;

public record DescriptionChanged(string Description) : IEvent;