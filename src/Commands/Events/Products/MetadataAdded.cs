namespace Commands.Events.Products;

public record MetadataAdded(Guid Id, Dictionary<string, string> Metadata);