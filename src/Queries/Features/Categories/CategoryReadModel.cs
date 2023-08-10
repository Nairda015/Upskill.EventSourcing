namespace Queries.Features.Categories;

public record CategoryReadModel(Guid Id, string Name, Guid? ParentId);