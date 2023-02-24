namespace Contracts.Events.Products;

// Is promo will help with tracking unfair promotions
public record PriceIncreased(decimal NewPrice, bool IsPromo);