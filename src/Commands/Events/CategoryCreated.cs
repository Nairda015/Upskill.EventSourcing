namespace Commands.Events;

public record CategoryCreated(Guid Id, string Name, Guid? ParentId);