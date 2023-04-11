using Microsoft.EntityFrameworkCore;

namespace Commands.Features.Categories;

public class CategoriesContext : DbContext
{
    public CategoriesContext(DbContextOptions<CategoriesContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = default!;
}