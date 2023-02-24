namespace Contracts.Events.Products;

public record PriceDecreased(decimal NewPrice, bool IsPromo);