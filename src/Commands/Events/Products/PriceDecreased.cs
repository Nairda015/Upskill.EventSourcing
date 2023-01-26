namespace Commands.Events.Products;

public record PriceDecreased(decimal NewPrice, bool IsPromo);