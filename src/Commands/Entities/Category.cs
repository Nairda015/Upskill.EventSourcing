namespace Commands.Entities;

/// <summary>
/// Category is fully immutable and stored only in sql database - Aurora
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="ParentId"></param>
public record Category(Guid Id, string Name, Guid? ParentId);