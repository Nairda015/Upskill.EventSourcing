namespace Commands.Events.Products;

public record PriceIncreased(Guid Id, decimal NewPrice);