namespace Contracts.Events.Products;

public record CategoryChanged(Guid CategoryId) : ISqsMessage;