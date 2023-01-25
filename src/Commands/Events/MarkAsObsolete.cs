namespace Commands.Events;

public record MarkAsObsolete(Guid Id);

// for metric only - no need for storing in Elastic
// flow: Lambda -> Dynamo -> CloudWatch
public record CategoryCreated(Guid Id, string Name, Guid? ParentId);

public record ProductCreated(Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string Description,
    Dictionary<string, string> Metadata); //metadata is wired in this context - something like tags?