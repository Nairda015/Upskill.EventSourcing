namespace Commands.Events;

public record MarkAsObsolete(Guid Id);

// for metric only - no need for storing in Elastic
// flow: Lambda -> Dynamo -> CloudWatch
public record CategoryCreated(Guid Id, string Name, Guid? ParentId);