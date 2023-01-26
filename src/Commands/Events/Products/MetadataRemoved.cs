namespace Commands.Events.Products;

public record MetadataRemoved(Guid Id, Dictionary<string, string> Metadata);