namespace Commands.Events.Categories;

public record CategoryCreated(Guid Id, string Name, Guid? ParentId);