namespace Queries.Models;

public record CategoryReadModel(Guid Id, string Name, Guid? ParentId);