using Commands.Entities;
using Microsoft.EntityFrameworkCore;

namespace Commands.Persistence;

public class CategoriesContext : DbContext
{
    public CategoriesContext(DbContextOptions<CategoriesContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = default!;
}